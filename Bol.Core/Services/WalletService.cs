using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bol.Address;
using Bol.Address.Abstractions;
using Bol.Core.Abstractions;
using Bol.Core.Model;
using Bol.Cryptography;

namespace Bol.Core.Services
{
    public class WalletService : IWalletService
    {
        private static readonly SCrypt s_defaultScript = new SCrypt{ N = 16384, R = 8, P = 8,};
        
        private readonly IAddressService _addressService;
        private readonly IKeyPairFactory _keyPairFactory;
        private readonly ISha256Hasher _sha256Hasher;
        private readonly ISignatureScriptFactory _signatureScriptFactory;
        private readonly IAddressTransformer _addressTransformer;
        private readonly IExportKeyFactory _exportKeyFactory;
        private readonly IBase16Encoder _base16Encoder;

        public WalletService(
            IAddressService addressService,
            ISignatureScriptFactory signatureScriptFactory,
            IKeyPairFactory keyPairFactory,
            ISha256Hasher sha256Hasher,
            IAddressTransformer addressTransformer,
            IExportKeyFactory exportKeyFactory,
            IBase16Encoder base16Encoder)
        {
            _addressService = addressService ?? throw new ArgumentNullException(nameof(addressService));
            _keyPairFactory = keyPairFactory ?? throw new ArgumentNullException(nameof(keyPairFactory));
            _sha256Hasher = sha256Hasher ?? throw new ArgumentNullException(nameof(sha256Hasher));
            _signatureScriptFactory = signatureScriptFactory ?? throw new ArgumentNullException(nameof(signatureScriptFactory));
            _addressTransformer = addressTransformer ?? throw new ArgumentNullException(nameof(addressTransformer));
            _exportKeyFactory = exportKeyFactory ?? throw new ArgumentNullException(nameof(exportKeyFactory));
            _base16Encoder = base16Encoder ?? throw new ArgumentNullException(nameof(base16Encoder));
        }

        public Task<BolWallet> CreateWalletB(string walletPassword, string codeName, string edi, string privateKey = null, CancellationToken token = default)
        {
            return CreateWallet(_addressService.GenerateAddressBAsync, walletPassword, codeName, edi, privateKey, token);
        }

        public Task<BolWallet> CreateWalletC(string walletPassword, string codeName, string edi, string privateKey = null, CancellationToken token = default)
        {
            return CreateWallet(_addressService.GenerateAddressCAsync, walletPassword, codeName, edi, privateKey, token);
        }

        private async Task<BolWallet> CreateWallet(Func<string,IKeyPair,CancellationToken,Task<BolAddress>> bolAddressGenerator, string walletPassword, string codeName, string edi, string privateKey = null, CancellationToken token = default)
        {
            var privateKeyPair = privateKey == null
                ? _keyPairFactory.Create()
                : _keyPairFactory.Create(_base16Encoder.Decode(privateKey));
            
            var codeNameKeyPair = _keyPairFactory.Create(_sha256Hasher.Hash(Encoding.ASCII.GetBytes(codeName)));
            
            var bolAddress = await bolAddressGenerator(codeName, privateKeyPair, token);
            var nonce = BitConverter.GetBytes(bolAddress.Nonce);
            
            var extendedKeyPair = _keyPairFactory.Create(_sha256Hasher.Hash(privateKeyPair.PrivateKey.Concat(nonce)));
            
            var mainScript = _signatureScriptFactory.Create(new[] { codeNameKeyPair.PublicKey, extendedKeyPair.PublicKey }, 2);
            var mainAccount = CreateAccount(walletPassword, "main", mainScript, multiSig: true);
            mainAccount.IsDefault = true;

            var codeNameTask = Task.Run(() => CreateAccount(walletPassword, "codename", privateKey: codeNameKeyPair.PrivateKey), token);
            var privateKeyAccountTask = Task.Run(() => CreateAccount(walletPassword, "private", privateKey: extendedKeyPair.PrivateKey), token);
            var blockchainAccountTask = Task.Run(() => CreateAccount(walletPassword, "blockchain"), token);
            var socialAccounTask = Task.Run(() => CreateAccount(walletPassword, "social"), token);
            var votingAccountTask = Task.Run(() => CreateAccount(walletPassword, "voting"), token);
            var commercialAccountsTask = Enumerable.Range(0, 12)
                .Select(_ => Task.Run(() => CreateAccount(walletPassword, "commercial"), token))
                .ToArray();

            var tasks = new List<Task<Account>>
            {
                codeNameTask, privateKeyAccountTask, blockchainAccountTask, socialAccounTask, votingAccountTask
            };
            tasks.AddRange(commercialAccountsTask);

            await Task.WhenAll(tasks);

            var codeNameAccount = codeNameTask.Result;
            codeNameAccount.Extra = new Extra { codename = codeName, edi = edi };

            var privateKeyAccount = privateKeyAccountTask.Result;
            privateKeyAccount.Extra = new Extra { nonce = bolAddress.Nonce.ToString() };

            var blockchainAccount = blockchainAccountTask.Result;
            var socialAccount = socialAccounTask.Result;
            var votingAccount = votingAccountTask.Result;
            var commercialAccounts = commercialAccountsTask.Select(x => x.Result).ToArray();

            var bolWallet = new BolWallet
            {
                Name = codeName,
                Version = "1.0",
                Scrypt = s_defaultScript,
                accounts = new[]
                    {
                        mainAccount, 
                        codeNameAccount, 
                        privateKeyAccount, 
                        blockchainAccount, 
                        socialAccount,
                        votingAccount
                    }
                    .Concat(commercialAccounts)
                    .ToArray()
            };

            return bolWallet;
        }

        private Account CreateAccount(string password, string label, ISignatureScript signatureScript = null, byte[] privateKey = null, bool multiSig = false)
        {
            var keyPair = privateKey == null 
                ? _keyPairFactory.Create() 
                : _keyPairFactory.Create(privateKey);
            
            signatureScript ??= _signatureScriptFactory.Create(keyPair.PublicKey);
            
            var encryptedKey = multiSig 
                ? null 
                : _exportKeyFactory.Export(keyPair.PrivateKey, signatureScript.ToScriptHash(), password, s_defaultScript.N, s_defaultScript.R, s_defaultScript.P);
            
            var contract = new Contract
            {
                Script = signatureScript.ToHexString(),
                Parameters = AccountParameters(multiSig),
                Deployed = false
            };
            
            var account = new Account
            {
                Address = _addressTransformer.ToAddress(signatureScript.ToScriptHash()),
                Label = label,
                IsDefault = false,
                Lock = false,
                Key = encryptedKey,
                Contract = contract,
                Extra = null
            };
            
            return account;
        }

        private IEnumerable<Parameter> AccountParameters(bool multiSig)
        {
            return multiSig
                ? new[]
                {
                    new Parameter() { Type = "Signature", Name = "parameter0" },
                    new Parameter() { Type = "Signature", Name = "parameter1" },
                }
                : new[] { new Parameter() { Type = "Signature", Name = "signature" } };
        }
    }
}
