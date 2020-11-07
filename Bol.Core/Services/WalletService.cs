using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bol.Address;
using Bol.Address.Abstractions;
using Bol.Core.Abstractions;
using Bol.Core.Model.Wallet;
using Bol.Cryptography;

namespace Bol.Core.Services
{
    public class WalletService : IWalletService
    {
        private IAddressService _addressService;
        private IKeyPairFactory _keyPairFactory;
        private ISha256Hasher _sha256Hasher;
        private ISignatureScriptFactory _signatureScriptFactory;
        private IAddressTransformer _addressTransformer;
        private IExportKeyFactory _exportKeyFactory;
        private IBase16Encoder _base16Encoder;

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


        public async Task<BolWallet> CreateWallet(string walletPassword, string codeName, string edi, string privateKey = null, CancellationToken token = default)
        {
            var accounts = new List<Account>();

            var _scrypt = new SCrypt
            {
                N = 16384,
                R = 8,
                P = 8,
            };

            //codename
            var codeNamePrivateKey = _sha256Hasher.Hash(Encoding.ASCII.GetBytes(codeName));
            var CodeNamekeyPair = _keyPairFactory.Create(codeNamePrivateKey);
            var codeNameSignatureScript = _signatureScriptFactory.Create(CodeNamekeyPair.PublicKey);

            var _accountCodeName = CreateAccount(codeNameSignatureScript, codeNamePrivateKey, privateKey, _scrypt);
            _accountCodeName.Label = "codename";
            var _extraCodeName = new Extra
            {
                codename = codeName,
                edi = edi
            };
            _accountCodeName.Extra = _extraCodeName;
            accounts.Add(_accountCodeName);

            //private
            var privateKeyPair = _keyPairFactory.Create(_base16Encoder.Decode(privateKey));
            //Get Bol Address
            var bolAddress = await _addressService.GenerateAddressBAsync(codeName, privateKeyPair, token);
            var nonce = BitConverter.GetBytes(bolAddress.Nonce);

            var extendedPrivateKey = _sha256Hasher.Hash(privateKeyPair.PrivateKey.Concat(nonce));
            var extendedKeyPair = _keyPairFactory.Create(extendedPrivateKey);
            var extendedSignatureScript = _signatureScriptFactory.Create(extendedKeyPair.PublicKey);


            var _accountprivate = CreateAccount(extendedSignatureScript, extendedPrivateKey, privateKey, _scrypt);
            _accountprivate.Label = "private";
            var _extraPrivate = new Extra
            {
                nonce = bolAddress.Nonce.ToString()
            };
            _accountprivate.Extra = _extraPrivate;
            accounts.Add(_accountprivate);

            //multisig
            var multisigList = new[] { CodeNamekeyPair.PublicKey, extendedKeyPair.PublicKey };

            var multisigSignatureScript = _signatureScriptFactory.Create(multisigList, 2);
            var _accountMultisig = CreateAccount(multisigSignatureScript, null, privateKey, _scrypt);
            _accountMultisig.Label = "main";
            _accountMultisig.IsDefault = true;
            accounts.Add(_accountMultisig);

            // blockchain
            var blockchainkeyPair = _keyPairFactory.Create();
            var blockchainSignatureScript = _signatureScriptFactory.Create(blockchainkeyPair.PublicKey);
            var _accountBlockchain = CreateAccount(blockchainSignatureScript, blockchainkeyPair.PrivateKey, privateKey, _scrypt);
            _accountBlockchain.Label = "blockchain";
            accounts.Add(_accountBlockchain);

            // social
            var socialkeyPair = _keyPairFactory.Create();
            var socialSignatureScript = _signatureScriptFactory.Create(socialkeyPair.PublicKey);
            var _accountSocial = CreateAccount(socialSignatureScript, socialkeyPair.PrivateKey, privateKey, _scrypt);
            _accountSocial.Label = "social";
            accounts.Add(_accountSocial);

            //commercial
            for (var i = 0; i < 9; i++)
            {
                var commercialkeyPair = _keyPairFactory.Create();
                var commercialSignatureScript = _signatureScriptFactory.Create(commercialkeyPair.PublicKey);
                var _commercialSocial = CreateAccount(commercialSignatureScript, commercialkeyPair.PrivateKey, privateKey, _scrypt);
                _commercialSocial.Label = "commercial";
                accounts.Add(_commercialSocial);
            }

            //Create Bol Wallet
            var bolWallet = new BolWallet
            {
                Name = codeName,
                Version = "1.0",
                Scrypt = _scrypt,
                accounts = accounts
            };

            return bolWallet;
        }

        private Account CreateAccount(ISignatureScript signatureScript, byte[] privateKey, string password, SCrypt scrypt)
        {
            var key = "";
            var parametersList = new List<Parameters>();
            if (privateKey == null) // multisig
            {
                key = null;
                parametersList = new List<Parameters>() {
                    new Parameters() { Type = "Signature", Name = "parameter0"} ,
                     new Parameters() { Type = "Signature", Name = "parameter1"} ,
                };
            }
            else  //all the others
            {
                key = _exportKeyFactory.Export(privateKey, signatureScript.ToScriptHash(), password, scrypt.N, scrypt.R, scrypt.P);
                parametersList = new List<Parameters>() {
                    new Parameters() { Type = "Signature", Name = "signature"} ,
                };
            }
            var _contract = new Contract
            {
                Script = signatureScript.ToHexString(),
                Parameters = parametersList,
                Deployed = false
            };
            var _accounts = new Account
            {
                Address = _addressTransformer.ToAddress(signatureScript.ToScriptHash()),
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
