using Bol.Core.Abstractions;
using Neo;
using Neo.Cryptography;
using Neo.IO.Json;
using Neo.SmartContract;
using Neo.Wallets;
using Neo.Wallets.NEP6;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bol.Core.Services
{
    public class WalletService : IWalletService
    {
        private IAddressService _addressService;
        private WalletIndexer _walletIndexer;

        public WalletService(IAddressService addressService, WalletIndexer walletIndexer)
        {
            _addressService = addressService ?? throw new ArgumentNullException(nameof(addressService));
            _walletIndexer = walletIndexer ?? throw new ArgumentNullException(nameof(walletIndexer));
        }

        public async Task<NEP6Wallet> CreateWallet(string walletPassword, string codeName, string edi, string privateKey = null, CancellationToken token = default)
        {
            var wallet = new NEP6Wallet(_walletIndexer, $"{codeName}.json", codeName);
            wallet.Unlock(walletPassword);

            var codeNamePrivateKey = Encoding.ASCII.GetBytes(codeName).Sha256();
            var codeNameAccount = (NEP6Account)wallet.CreateAccount(codeNamePrivateKey);
            codeNameAccount.Label = "codename";
            var extraCodeName = new JObject();
            extraCodeName["codename"] = codeName;
            extraCodeName["edi"] = edi;
            codeNameAccount.Extra = extraCodeName;

            var privateKeyPair = new KeyPair(privateKey.HexToBytes());

            var bolAddress = await _addressService.GenerateAddressBAsync(codeName, privateKeyPair, token);
            var nonce = BitConverter.GetBytes(bolAddress.Nonce);

            var extendedPrivateKey = privateKeyPair.PrivateKey.Concat(nonce).Sha256();
            var privateAccount = (NEP6Account)wallet.CreateAccount(extendedPrivateKey);
            privateAccount.Label = "private";
            var extraPrivate = new JObject();
            extraPrivate["nonce"] = bolAddress.Nonce;
            privateAccount.Extra = extraPrivate;

            var multisig = Contract.CreateMultiSigContract(2, codeNameAccount.GetKey().PublicKey, privateAccount.GetKey().PublicKey);

            var bAddressAccount = wallet.CreateAccount(multisig);
            bAddressAccount.Label = "main";
            bAddressAccount.IsDefault = true;

            var blockchainAccount = (NEP6Account)wallet.CreateAccount();
            blockchainAccount.Label = "blockchain";

            var socialAccount = (NEP6Account)wallet.CreateAccount();
            socialAccount.Label = "social";

            for (var i = 0; i < 9; i++)
            {
                var commercialAccount = (NEP6Account)wallet.CreateAccount();
                commercialAccount.Label = "commercial";
            }

            return wallet;
        }
    }
}
