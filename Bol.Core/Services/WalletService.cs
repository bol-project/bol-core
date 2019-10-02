using Bol.Core.Abstractions;
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

        public async Task<NEP6Wallet> CreateWallet(string codeName, byte[] privateKey, string walletPassword, CancellationToken token = default)
        {
            var wallet = new NEP6Wallet(_walletIndexer, $"{codeName}.json", codeName);
            wallet.Unlock(walletPassword);

            var codeNamePrivateKey = Encoding.ASCII.GetBytes(codeName).Sha256();
            var codeNameAccount = wallet.CreateAccount(codeNamePrivateKey);
            codeNameAccount.Label = "codename";

            var privateKeyPair = new KeyPair(privateKey);

            var bolAddress = await _addressService.GenerateAddressBAsync(codeName, privateKeyPair, token);
            var nonce = BitConverter.GetBytes(bolAddress.Nonce);

            var extendedPrivateKey = privateKeyPair.PrivateKey.Concat(nonce).Sha256();
            var privateAccount = (NEP6Account)wallet.CreateAccount(extendedPrivateKey);
            privateAccount.Label = "private";
            var extra = new JObject();
            extra["nonce"] = bolAddress.Nonce;
            privateAccount.Extra = extra;

            var multisig = Contract.CreateMultiSigContract(2, codeNameAccount.GetKey().PublicKey, privateAccount.GetKey().PublicKey);

            var bAddressAccount = wallet.CreateAccount(multisig);
            bAddressAccount.Label = "B";
            bAddressAccount.IsDefault = true;

            return wallet;
        }
    }
}
