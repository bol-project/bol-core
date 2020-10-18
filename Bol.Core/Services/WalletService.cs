using Bol.Core.Abstractions;
using Neo;
//using Neo.Cryptography;
using Neo.IO.Json;
using Neo.SmartContract;
using Neo.Wallets;
using Neo.Wallets.NEP6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bol.Core.Services
{
    public class WalletService : IWalletService
    {
        private IAddressService _addressService;
        private Cryptography.IKeyPairFactory _keyPairFactory;
        private Cryptography.ISha256Hasher _sha256Hasher;
        private Address.ISignatureScriptFactory _signatureScriptFactory;
        private Address.IAddressTransformer _addressTransformer;
        private Address.Abstractions.IExportKeyFactory _exportKeyFactory;
        private IJsonSerializer _jsonSerializer;


        public WalletService(
            IAddressService addressService,
            Address.ISignatureScriptFactory signatureScriptFactory,
            Cryptography.IKeyPairFactory keyPairFactory,
            Cryptography.ISha256Hasher sha256Hasher,
            Address.IAddressTransformer addressTransformer,
            Address.Abstractions.IExportKeyFactory exportKeyFactory,
            IJsonSerializer jsonSerializer)
        {
            _addressService = addressService ?? throw new ArgumentNullException(nameof(addressService));
            _keyPairFactory = keyPairFactory ?? throw new ArgumentNullException(nameof(keyPairFactory));
            _sha256Hasher = sha256Hasher ?? throw new ArgumentNullException(nameof(sha256Hasher));
            _signatureScriptFactory = signatureScriptFactory ?? throw new ArgumentNullException(nameof(signatureScriptFactory));
            _addressTransformer = addressTransformer ?? throw new ArgumentNullException(nameof(addressTransformer));
            _exportKeyFactory = exportKeyFactory ?? throw new ArgumentNullException(nameof(exportKeyFactory));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

    
        public async Task<string> CreateWallet(string walletPassword, string codeName, string edi, string privateKey = null, CancellationToken token = default)
        {
            List<Model.account> accounts = new List<Model.account>();
            var codeNamePrivateKey = _sha256Hasher.Hash(Encoding.ASCII.GetBytes(codeName));
            var key = _keyPairFactory.Create(codeNamePrivateKey);
            var ScriptHash = _signatureScriptFactory.Create(key.PublicKey).ToScriptHash();
            var _scrypt = new Model.scrypt
            {
                N = 16384,
                R = 8,
                P = 8,
            };
          
            var _parameters = new Model.parameters
            {
               Type = "Signature", 
               Name = "signature",
            };
            var _contract = new Model.contract
            {
               Script = ScriptHash.ToHexString(),
               Parameters = _parameters,
               Deployed = false
            };
            var _extra = new Model.extra
            {
                Codename= codeName,
                Edi = edi,
            };
            var _accounts = new Model.account
            {
                Address = _addressTransformer.ToAddress(ScriptHash),
                Label = codeName,
                IsDefault = false,
                Lock = false,
                Key = _exportKeyFactory.Export(codeNamePrivateKey, ScriptHash, privateKey, _scrypt.N, _scrypt.R, _scrypt.P),
                Contract = _contract,
                Extra = _extra
            };
            accounts.Add(_accounts);
            var _bolWallet = new Model.BolWallet {
               Name = codeName,
               Version = "1.0",
               Scrypt = _scrypt,          
               accounts = accounts
            };

           //  var wallet = new NEP6Wallet(_walletIndexer, $"{codeName}.json", codeName);
            // wallet.Unlock(walletPassword);

            //var codeNamePrivateKey = Encoding.ASCII.GetBytes(codeName).Sha256();
            //var codeNameAccount = (NEP6Account)wallet.CreateAccount(codeNamePrivateKey);
            //codeNameAccount.Label = "codename";
            //var extraCodeName = new JObject();
            //extraCodeName["codename"] = codeName;
            //extraCodeName["edi"] = edi;
            //codeNameAccount.Extra = extraCodeName;

            var privateKeyPair =  new Neo.Wallets.KeyPair(privateKey.HexToBytes());

            var bolAddress = await _addressService.GenerateAddressBAsync(codeName, privateKeyPair, token);
            var nonce = BitConverter.GetBytes(bolAddress.Nonce);

            // var extendedPrivateKey = privateKeyPair.PrivateKey.Concat(nonce).Sha256();
           // var privateAccount = (NEP6Account)wallet.CreateAccount(extendedPrivateKey);
            //privateAccount.Label = "private";
            //var extraPrivate = new JObject();
            //extraPrivate["nonce"] = bolAddress.Nonce;   
            //privateAccount.Extra = extraPrivate;

            //var multisig = Contract.CreateMultiSigContract(2, codeNameAccount.GetKey().PublicKey, privateAccount.GetKey().PublicKey);

            //var bAddressAccount = wallet.CreateAccount(multisig);
            //bAddressAccount.Label = "main";
            //bAddressAccount.IsDefault = true;

            //var blockchainAccount = (NEP6Account)wallet.CreateAccount();
            //blockchainAccount.Label = "blockchain";

            //var socialAccount = (NEP6Account)wallet.CreateAccount();
            //socialAccount.Label = "social";

            //for (var i = 0; i < 9; i++)
            //{
            //    var commercialAccount = (NEP6Account)wallet.CreateAccount();
            //    commercialAccount.Label = "commercial";
            //}

            return _jsonSerializer.Serialize(_bolWallet);
        }
    }
}
