using System.IO;
using Bol.Address;
using Bol.Address.Model.Configuration;
using Bol.Address.Neo;
using Bol.Core.Accessors;
using Bol.Core.Model;
using Bol.Core.Model.Wallet;
using Bol.Core.Services;
using Bol.Core.Transactions;
using Bol.Cryptography.Encoders;
using Bol.Cryptography.Hashers;
using Bol.Cryptography.Keys;
using Bol.Cryptography.Signers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Bol.Coin.Tests.Utils
{
    public static class BolServiceFactory
    {
        public static BolService Create(TransactionGrabber grabber, BolContext context)
        {
            var sha256 = new Sha256Hasher();
            var base16 = new Base16Encoder(sha256);
            var base58 = new Base58Encoder(sha256);
            var ripemd160 = new RipeMD160Hasher();

            var signatureScriptFactory = new SignatureScriptFactory(base16, sha256, ripemd160);
            var scriptHashFactory = new ScriptHashFactory(base16);
            var transactionSerializer = new TransactionSerializer();
            var transactionSigner = new TransactionSigner(transactionSerializer, new ECCurveSigner());
            var transactionNotarizer = new TransactionNotarizer(transactionSigner);
            
            var rpcMethodFactory = new FakeRpcMethodFactory(grabber);
            var transactionService = new TransactionService(signatureScriptFactory, scriptHashFactory, transactionNotarizer, rpcMethodFactory);

            var contextAccessor = new FakeContextAccessor(context);
            
            var protocolConfig = Options.Create(new ProtocolConfiguration { AddressVersion = "25" });
            var addressTransformer = new AddressTransformer(base58, base16, protocolConfig);
            
            return new BolService(contextAccessor, transactionService, signatureScriptFactory, base16, addressTransformer);
        }
        
        public static BolService Create(TransactionGrabber grabber)
        {
            var walletJson = File.ReadAllText("wallet.json");

            var wallet = Options.Create(JsonConvert.DeserializeObject<BolWallet>(walletJson));
            var walletConfig = Options.Create(new WalletConfiguration { Password = "bol" });

            var appsettingsJson = File.ReadAllText("appsettings.json");
            var definition = new { BolConfig = new BolConfig() };
            var bolConfig = Options.Create(JsonConvert.DeserializeAnonymousType(appsettingsJson, definition).BolConfig);

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

            var rpcMethodFactory = new FakeRpcMethodFactory(grabber);
            var transactionService = new TransactionService(signatureScriptFactory, scriptHashFactory, transactionNotarizer, rpcMethodFactory);

            return new BolService(contextAccessor, transactionService, signatureScriptFactory, base16, addressTransformer);
        }
    }
}
