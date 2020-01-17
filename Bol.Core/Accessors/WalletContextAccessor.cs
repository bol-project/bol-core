using System;
using System.Collections.Generic;
using System.Linq;
using Bol.Core.Abstractions;
using Bol.Core.Model;
using Neo;
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
            var mainAddressAccount = accounts.Where(account => account.Label == "main").Single();

            var blockchainAddressAccount = accounts.Where(account => account.Label == "blockchain").SingleOrDefault();
            var socialAddressAccount = accounts.Where(account => account.Label == "social").SingleOrDefault();
            var commercialAddressAccounts = accounts.Where(account => account.Label == "commercial");

            return new BolContext(
                codeNameAccount.Extra["codename"].AsString(),
                codeNameAccount.Extra["edi"].AsString(),
                new KeyPair(codeNameAccount.GetKey().PrivateKey),
                new KeyPair(privateAccount.GetKey().PrivateKey),
                mainAddressAccount.ScriptHash,
                new KeyValuePair<UInt160, KeyPair>(blockchainAddressAccount?.ScriptHash, blockchainAddressAccount?.GetKey()),
                new KeyValuePair<UInt160, KeyPair>(socialAddressAccount?.ScriptHash, socialAddressAccount?.GetKey()),
                commercialAddressAccounts.Select(account => new KeyValuePair<UInt160, KeyPair>(account.ScriptHash, account.GetKey())).ToList()
                );
        }
    }
}
