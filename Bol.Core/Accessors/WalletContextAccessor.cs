using System;
using System.Collections.Generic;
using System.Linq;
using Bol.Address;
using Bol.Address.Abstractions;
using Bol.Core.Abstractions;
using Bol.Core.Model;
using Bol.Core.Model.Wallet;
using Bol.Cryptography;
using Microsoft.Extensions.Options;

namespace Bol.Core.Accessors
{
    public class WalletContextAccessor : IContextAccessor
    {
        private readonly BolWallet _BolWallet;
        private readonly IExportKeyFactory _exportKeyFactory;
        private readonly IKeyPairFactory _keyPairFactory;
        private readonly IAddressTransformer _addressTransformer;

        public WalletContextAccessor(
            IOptions<BolWallet> bolWallet,
            IExportKeyFactory exportKeyFactory,
            IKeyPairFactory keyPairFactory,
            IAddressTransformer addressTransformer)
        {
            _BolWallet = bolWallet.Value ?? throw new ArgumentNullException(nameof(bolWallet));
            _exportKeyFactory = exportKeyFactory ?? throw new ArgumentNullException(nameof(exportKeyFactory));
            _keyPairFactory = keyPairFactory ?? throw new ArgumentNullException(nameof(keyPairFactory));
            _addressTransformer = addressTransformer ?? throw new ArgumentNullException(nameof(addressTransformer));
        }

        public BolContext GetContext()
        {
            var accounts = _BolWallet.accounts.Select(a => a as Account).ToList();

            var codeNameAccount = accounts.Where(account => account.Label == "codename").Single();
            var privateAccount = accounts.Where(account => account.Label == "private").Single();
            var mainAddressAccount = accounts.Where(account => account.Label == "main").Single();

            var blockchainAddressAccount = accounts.Where(account => account.Label == "blockchain").SingleOrDefault();
            var socialAddressAccount = accounts.Where(account => account.Label == "social").SingleOrDefault();
            var commercialAddressAccounts = accounts.Where(account => account.Label == "commercial");

            return new BolContext(
                codeNameAccount.Extra.codename,
                codeNameAccount.Extra.edi,
                _keyPairFactory.Create((_exportKeyFactory.GetDecryptedPrivateKey(codeNameAccount.Key, "bol", _BolWallet.Scrypt.N, _BolWallet.Scrypt.R, _BolWallet.Scrypt.P))),
                _keyPairFactory.Create((_exportKeyFactory.GetDecryptedPrivateKey(privateAccount.Key, "bol", _BolWallet.Scrypt.N, _BolWallet.Scrypt.R, _BolWallet.Scrypt.P))),
                _addressTransformer.ToScriptHash(mainAddressAccount.Address),
                new KeyValuePair<IScriptHash, IKeyPair>(_addressTransformer.ToScriptHash(blockchainAddressAccount.Address), _keyPairFactory.Create((_exportKeyFactory.GetDecryptedPrivateKey(blockchainAddressAccount.Key, "bol", _BolWallet.Scrypt.N, _BolWallet.Scrypt.R, _BolWallet.Scrypt.P)))),
                new KeyValuePair<IScriptHash, IKeyPair>(_addressTransformer.ToScriptHash(socialAddressAccount.Address), _keyPairFactory.Create((_exportKeyFactory.GetDecryptedPrivateKey(socialAddressAccount.Key, "bol", _BolWallet.Scrypt.N, _BolWallet.Scrypt.R, _BolWallet.Scrypt.P)))),
                commercialAddressAccounts.Select(account => new KeyValuePair<IScriptHash, IKeyPair>(_addressTransformer.ToScriptHash(account.Address), _keyPairFactory.Create((_exportKeyFactory.GetDecryptedPrivateKey(account.Key, "bol", _BolWallet.Scrypt.N, _BolWallet.Scrypt.R, _BolWallet.Scrypt.P))))).ToList()
                );
        }
    }
}
