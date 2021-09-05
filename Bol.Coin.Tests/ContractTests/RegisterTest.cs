using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Bol.Address;
using Bol.Address.Model.Configuration;
using Bol.Address.Neo;
using Bol.Core.Accessors;
using Bol.Core.Model;
using Bol.Core.Model.Wallet;
using Bol.Core.Rpc.Abstractions;
using Bol.Core.Services;
using Bol.Core.Transactions;
using Bol.Cryptography.Encoders;
using Bol.Cryptography.Hashers;
using Bol.Cryptography.Keys;
using Bol.Cryptography.Signers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Neo.Emulation;
using Neo.Emulation.API;
using Neo.VM;
using Newtonsoft.Json;
using Xunit;

namespace Bol.Coin.Tests.ContractTests
{
    public class TransactionGrabber : IRpcMethodFactory
    {
        public BolTransaction Transaction { get; set; }

        public Task<T> GetAccount<T>(string mainAddress, CancellationToken token = default)
        {
            return Task.FromResult(default(T));
        }

        public Task<T> SendRawTransaction<T>(BolTransaction transaction, CancellationToken token = default)
        {
            Transaction = transaction;
            return Task.FromResult(default(T));
        }

        public Task<T> TestRawTransaction<T>(BolTransaction transaction, CancellationToken token = default)
        {
            Transaction = transaction;
            return Task.FromResult(default(T));
        }
    }

    public class RegisterTest
    {
        private Emulator _emulator;
        private BolService _service;
        private TransactionGrabber _transactionGrabber;

        public RegisterTest()
        {
            var avmBytes = File.ReadAllBytes("Bol.Coin.avm");
            var chain = new Blockchain();
            _emulator = new Emulator(chain);

            var address = chain.DeployContract("BoL", avmBytes);
            _emulator.SetExecutingAccount(address);

            var walletJson = File.ReadAllText("wallet.json");

            var wallet = Options.Create(JsonConvert.DeserializeObject<BolWallet>(walletJson));
            var walletConfig = Options.Create(new WalletConfiguration { Password = "bol" });
            var bolConfig = Options.Create(new BolConfig { Contract = "49071c33087967cc6d3c0f0ef35c013163b047eb" });
            var protocolConfig = Options.Create(new ProtocolConfiguration { AddressVersion = "25" });

            var sha256 = new Sha256Hasher();
            var xor = new Xor();
            var base58 = new Base58Encoder(sha256);
            var base16 = new Base16Encoder(sha256);
            var ripemd160 = new RipeMD160Hasher();

            var addressTransformer = new AddressTransformer(base58, base16, protocolConfig);

            var exportKeyFactory = new ExportKeyFactory(sha256, addressTransformer, xor);
            var keyPairFactory = new KeyPairFactory();
            var cachingService = new CachingService(new MemoryCache(Options.Create(new MemoryCacheOptions())));
            var signatureScriptFactory = new SignatureScriptFactory(base16, sha256, ripemd160);
            var scriptHashFactory = new ScriptHashFactory(base16);
            var transactionSerializer = new TransactionSerializer();
            var transactionSigner = new TransactionSigner(transactionSerializer, new ECCurveSigner());
            var transactionNotarizer = new TransactionNotarizer(transactionSigner);

            var contextAccessor = new WalletContextAccessor(wallet, walletConfig, bolConfig, exportKeyFactory, keyPairFactory, addressTransformer, cachingService);

            _transactionGrabber = new TransactionGrabber();
            var transactionService = new TransactionService(signatureScriptFactory, scriptHashFactory, transactionNotarizer, _transactionGrabber);

            _service = new BolService(contextAccessor, transactionService, signatureScriptFactory, base16);
        }

        [Fact]
        public async Task TestRegister()
        {
            var notifyOutput = "";
            Runtime.OnLogMessage = (string x) =>
            {
                notifyOutput = x;
            };
            await _service.Deploy();
            var loaderScript = _transactionGrabber.Transaction.ExecutionScript.GetBytes();

            _emulator.checkWitnessMode = CheckWitnessMode.AlwaysTrue;
            _emulator.Reset(loaderScript, null, "deploy", table: new ScriptTable());
            _emulator.Run();

            await _service.Register();

            loaderScript = _transactionGrabber.Transaction.ExecutionScript.GetBytes();

            _emulator.checkWitnessMode = CheckWitnessMode.AlwaysTrue;
            _emulator.Reset(loaderScript, null, "register", table: new ScriptTable());
            _emulator.Run();

            var result = _emulator.GetOutput();
            Assert.NotNull(result);

            var register = result.GetBoolean();
            Assert.True(register);

            var notifyOutputObject = notifyOutput.ToString();
        }
    }


}
