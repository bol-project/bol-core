using System;
using System.Linq;
using Bol.Core.Abstractions;
using Bol.Core.Model;
using Neo.Wallets;
using Neo.Wallets.NEP6;

namespace Bol.Core.Accessors
{
    public class WalletContextAccessor : IContextAccessor
    {
        private NEP6Wallet _wallet;

        public WalletContextAccessor(NEP6Wallet wallet)
        {
            _wallet = wallet ?? throw new ArgumentNullException(nameof(wallet));
        }

        public BolContext GetContext()
        {
            var accounts = _wallet.GetAccounts()
                .Select(a => a as NEP6Account)
                .ToList();

            var codeNameAccount = accounts.Where(account => account.Label == "codename").Single();
            var privateAccount = accounts.Where(account => account.Label == "private").Single();
            var bAddressAccount = accounts.Where(account => account.Label == "B").Single();

            return new BolContext(
                codeNameAccount.Extra["codename"].AsString(),
                codeNameAccount.Extra["edi"].AsString(),
                new KeyPair(codeNameAccount.GetKey().PrivateKey),
                new KeyPair(privateAccount.GetKey().PrivateKey),
                bAddressAccount.ScriptHash
                );
        }
    }
}
