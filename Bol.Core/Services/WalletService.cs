using Bol.Address;
using Bol.Core.Abstractions;
using Bol.Core.Model;
using Bol.Cryptography;
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
        private Cryptography.Abstractions.IHexToBytesFactory _hexToBytesFactory;

        public WalletService(
            IAddressService addressService,
            Address.ISignatureScriptFactory signatureScriptFactory,
            Cryptography.IKeyPairFactory keyPairFactory,
            Cryptography.ISha256Hasher sha256Hasher,
            Address.IAddressTransformer addressTransformer,
            Address.Abstractions.IExportKeyFactory exportKeyFactory,
            IJsonSerializer jsonSerializer,
            Cryptography.Abstractions.IHexToBytesFactory hexToBytesFactory)
        {
            _addressService = addressService ?? throw new ArgumentNullException(nameof(addressService));
            _keyPairFactory = keyPairFactory ?? throw new ArgumentNullException(nameof(keyPairFactory));
            _sha256Hasher = sha256Hasher ?? throw new ArgumentNullException(nameof(sha256Hasher));
            _signatureScriptFactory = signatureScriptFactory ?? throw new ArgumentNullException(nameof(signatureScriptFactory));
            _addressTransformer = addressTransformer ?? throw new ArgumentNullException(nameof(addressTransformer));
            _exportKeyFactory = exportKeyFactory ?? throw new ArgumentNullException(nameof(exportKeyFactory));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
            _hexToBytesFactory= hexToBytesFactory ?? throw new ArgumentNullException(nameof(hexToBytesFactory));
        }

    
        public async Task<string> CreateWallet(string walletPassword, string codeName, string edi, string privateKey = null, CancellationToken token = default)
        {
            List<Model.account> accounts = new List<Model.account>();

            var _scrypt = new Model.scrypt
            {
                N = 16384,
                R = 8,
                P = 8,
            };

            //codename
            var codeNamePrivateKey = _sha256Hasher.Hash(Encoding.ASCII.GetBytes(codeName));
            var CodeNamekeyPair = _keyPairFactory.Create(codeNamePrivateKey);
            var codeNameSignatureScript = _signatureScriptFactory.Create(CodeNamekeyPair.PublicKey);
          
            account _accountCodeName = CreateAccount(codeNameSignatureScript, codeNamePrivateKey, privateKey, _scrypt);
            _accountCodeName.Label = "codename";
            var _extraCodeName = new JObject();
            _extraCodeName["codename"] = codeName;
            _extraCodeName["edi"] = edi;
            _accountCodeName.Extra = _extraCodeName;
            accounts.Add(_accountCodeName);

            //private
            var privateKeyPair = _keyPairFactory.Create(_hexToBytesFactory.HexToBytesCreate(privateKey));
            //Get Bol Address
            var bolAddress= await _addressService.GenerateAddressBAsync(codeName, privateKeyPair, token);
            var nonce = BitConverter.GetBytes(bolAddress.Nonce);

            var extendedPrivateKey = _sha256Hasher.Hash(privateKeyPair.PrivateKey.Concat(nonce));
            var extendedKeyPair = _keyPairFactory.Create(extendedPrivateKey);
            var extendedSignatureScript = _signatureScriptFactory.Create(extendedKeyPair.PublicKey);


            account _accountprivate = CreateAccount(extendedSignatureScript, extendedPrivateKey, privateKey, _scrypt);
            _accountprivate.Label = "private";
            var _extraPrivate = new JObject();
            _extraPrivate["nonce"] = bolAddress.Nonce;
            _accountprivate.Extra = _extraPrivate;
            accounts.Add(_accountprivate);

            //multisig
            List<IPublicKey> multisigList = new List<IPublicKey>();
            multisigList.Add(CodeNamekeyPair.PublicKey);
            multisigList.Add(extendedKeyPair.PublicKey);

            var multisigSignatureScript = _signatureScriptFactory.Create(multisigList, 2);
            account _accountMultisig = CreateAccount(multisigSignatureScript, null, privateKey, _scrypt);
            _accountMultisig.Label = "main";
            _accountMultisig.IsDefault = true;
            accounts.Add(_accountMultisig);

            // blockchain
            var blockchainkeyPair = _keyPairFactory.Create();
            var blockchainSignatureScript = _signatureScriptFactory.Create(blockchainkeyPair.PublicKey);
            account _accountBlockchain = CreateAccount(blockchainSignatureScript, blockchainkeyPair.PrivateKey, privateKey, _scrypt);
            _accountBlockchain.Label = "blockchain";
            accounts.Add(_accountBlockchain);

            // social
            var socialkeyPair = _keyPairFactory.Create();
            var socialSignatureScript = _signatureScriptFactory.Create(socialkeyPair.PublicKey);
            account _accountSocial = CreateAccount(socialSignatureScript, socialkeyPair.PrivateKey, privateKey, _scrypt);
            _accountSocial.Label = "social";
            accounts.Add(_accountSocial);

            //commercial
            for (var i = 0; i < 9; i++)
            {
                var commercialkeyPair = _keyPairFactory.Create();
                var commercialSignatureScript = _signatureScriptFactory.Create(commercialkeyPair.PublicKey);
                account _commercialSocial = CreateAccount(commercialSignatureScript, commercialkeyPair.PrivateKey, privateKey, _scrypt);
                _commercialSocial.Label = "commercial";
                accounts.Add(_commercialSocial);
            }
            //Create Bol Wallet
            var _bolWallet = new Model.BolWallet
            { 
                Name = codeName,
                Version = "1.0",
                Scrypt = _scrypt,
                accounts = accounts
            };
            //  var wallet = new NEP6Wallet(_walletIndexer, $"{codeName}.json", codeName);
            // wallet.Unlock(walletPassword);

           //  var codeNamePrivateKey = Encoding.ASCII.GetBytes(codeName).Sha256();
           // var codeNameAccount = (NEP6Account)wallet.CreateAccount(codeNamePrivateKey);
            //codeNameAccount.Label = "codename";
            //var extraCodeName = new JObject();
            //extraCodeName["codename"] = codeName;
            //extraCodeName["edi"] = edi;
            //codeNameAccount.Extra = extraCodeName;

            // var privateKeyPair =  new Neo.Wallets.KeyPair(privateKey.HexToBytes());

            //var bolAddress = await _addressService.GenerateAddressBAsync(codeName, privateKeyPair, token);
            //var nonce = BitConverter.GetBytes(bolAddress.Nonce);

             //var extendedPrivateKey = privateKeyPair.PrivateKey.Concat(nonce).Sha256();
           //  var privateAccount = (NEP6Account)wallet.CreateAccount(extendedPrivateKey);
            //privateAccount.Label = "private";
            //var extraPrivate = new JObject();
            //extraPrivate["nonce"] = bolAddress.Nonce;   
            //privateAccount.Extra = extraPrivate;

          // var multisig = Contract.CreateMultiSigContract(2, codeNameAccount.GetKey().PublicKey, privateAccount.GetKey().PublicKey);

           // var bAddressAccount = wallet.CreateAccount(multisig);
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
        private account CreateAccount(Address.ISignatureScript SignatureScript, byte[] PrivateKey, string password, scrypt scrypt)
        {
            var key = "";
            IList<parameters> parametersList = new List<parameters>();
            if (PrivateKey == null) // multisig
            {
                key = null;
                parametersList = new List<parameters>() {
                    new parameters() { Type = "Signature", Name = "parameter0"} ,
                     new parameters() { Type = "Signature", Name = "parameter1"} ,
                };
            }
            else  //all the others
            {
                key = _exportKeyFactory.Export(PrivateKey, SignatureScript.ToScriptHash(), password, scrypt.N, scrypt.R, scrypt.P);
                parametersList = new List<parameters>() {
                    new parameters() { Type = "Signature", Name = "signature"} ,
                };
            }           
            var _contract = new Model.contract
            {
                Script = SignatureScript.ToHexString(),
                Parameters = parametersList,
                Deployed = false
            };
            var _accounts = new Model.account
            {
                Address = _addressTransformer.ToAddress(SignatureScript.ToScriptHash()),
                Label = "",
                IsDefault = false,
                Lock = false,
                Key = key,
                Contract = _contract,
                Extra = null
            };
            return _accounts;
        }
    }
}
