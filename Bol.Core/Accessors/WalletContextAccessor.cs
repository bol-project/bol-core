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
        private readonly BolWallet _bolWallet;
        private readonly WalletConfiguration _walletConfig;
        private readonly IExportKeyFactory _exportKeyFactory;
        private readonly IKeyPairFactory _keyPairFactory;
        private readonly IAddressTransformer _addressTransformer;

        public WalletContextAccessor(
            IOptions<BolWallet> bolWallet,
            IOptions<WalletConfiguration> walletConfig,
            IExportKeyFactory exportKeyFactory,
            IKeyPairFactory keyPairFactory,
            IAddressTransformer addressTransformer)
        {
            _bolWallet = bolWallet.Value ?? throw new ArgumentNullException(nameof(bolWallet));
            _walletConfig = walletConfig.Value ?? throw new ArgumentNullException(nameof(walletConfig));
            _exportKeyFactory = exportKeyFactory ?? throw new ArgumentNullException(nameof(exportKeyFactory));
            _keyPairFactory = keyPairFactory ?? throw new ArgumentNullException(nameof(keyPairFactory));
            _addressTransformer = addressTransformer ?? throw new ArgumentNullException(nameof(addressTransformer));
        }

        public BolContext GetContext()
        {
            var accounts = _bolWallet.accounts.Select(a => a as Account).ToList();
            var password = _walletConfig.Password;

            var codeNameAccount = accounts.Where(account => account.Label == "codename").Single();
            var privateAccount = accounts.Where(account => account.Label == "private").Single();
            var mainAddressAccount = accounts.Where(account => account.Label == "main").Single();

            var blockchainAddressAccount = accounts.Where(account => account.Label == "blockchain").SingleOrDefault();
            var socialAddressAccount = accounts.Where(account => account.Label == "social").SingleOrDefault();
            var commercialAddressAccounts = accounts.Where(account => account.Label == "commercial");

            return new BolContext(
                codeNameAccount.Extra.codename,
                codeNameAccount.Extra.edi,
                _keyPairFactory.Create((_exportKeyFactory.GetDecryptedPrivateKey(codeNameAccount.Key, password, _bolWallet.Scrypt.N, _bolWallet.Scrypt.R, _bolWallet.Scrypt.P))),
                _keyPairFactory.Create((_exportKeyFactory.GetDecryptedPrivateKey(privateAccount.Key, password, _bolWallet.Scrypt.N, _bolWallet.Scrypt.R, _bolWallet.Scrypt.P))),
                _addressTransformer.ToScriptHash(mainAddressAccount.Address),
                new KeyValuePair<IScriptHash, IKeyPair>(_addressTransformer.ToScriptHash(blockchainAddressAccount.Address), _keyPairFactory.Create((_exportKeyFactory.GetDecryptedPrivateKey(blockchainAddressAccount.Key, password, _bolWallet.Scrypt.N, _bolWallet.Scrypt.R, _bolWallet.Scrypt.P)))),
                new KeyValuePair<IScriptHash, IKeyPair>(_addressTransformer.ToScriptHash(socialAddressAccount.Address), _keyPairFactory.Create((_exportKeyFactory.GetDecryptedPrivateKey(socialAddressAccount.Key, password, _bolWallet.Scrypt.N, _bolWallet.Scrypt.R, _bolWallet.Scrypt.P)))),
                commercialAddressAccounts.Select(account => new KeyValuePair<IScriptHash, IKeyPair>(_addressTransformer.ToScriptHash(account.Address), _keyPairFactory.Create((_exportKeyFactory.GetDecryptedPrivateKey(account.Key, password, _bolWallet.Scrypt.N, _bolWallet.Scrypt.R, _bolWallet.Scrypt.P)))))
                );
        }
    }
}
