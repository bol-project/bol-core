using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bol.Address;
using Bol.Address.Abstractions;
using Bol.Core.Abstractions;
using Bol.Core.Model;
using Bol.Cryptography;
using Microsoft.Extensions.Options;

namespace Bol.Core.Accessors
{
    public class WalletContextAccessor : IContextAccessor
    {
        private readonly BolWallet _bolWallet;
        private readonly WalletConfiguration _walletConfig;
        private readonly BolConfig _bolConfig;
        private readonly IExportKeyFactory _exportKeyFactory;
        private readonly IKeyPairFactory _keyPairFactory;
        private readonly IAddressTransformer _addressTransformer;
        private readonly ICachingService _iCachingService;
        
        public WalletContextAccessor(
            IOptions<BolWallet> bolWallet,
            IOptions<WalletConfiguration> walletConfig,
            IOptions<BolConfig> bolConfig,
            IExportKeyFactory exportKeyFactory,
            IKeyPairFactory keyPairFactory,
            IAddressTransformer addressTransformer,
            ICachingService iCachingService)
        {
            _bolWallet = bolWallet.Value ?? throw new ArgumentNullException(nameof(bolWallet));
            _walletConfig = walletConfig.Value ?? throw new ArgumentNullException(nameof(walletConfig));
            _bolConfig = bolConfig.Value ?? throw new ArgumentNullException(nameof(bolConfig));
            _exportKeyFactory = exportKeyFactory ?? throw new ArgumentNullException(nameof(exportKeyFactory));
            _keyPairFactory = keyPairFactory ?? throw new ArgumentNullException(nameof(keyPairFactory));
            _addressTransformer = addressTransformer ?? throw new ArgumentNullException(nameof(addressTransformer));
            _iCachingService = iCachingService ?? throw new ArgumentNullException(nameof(iCachingService));
        }

        private IBolContext CreateContext()
        {
            var n = _bolWallet.Scrypt.N;
            var r = _bolWallet.Scrypt.R;
            var p = _bolWallet.Scrypt.P;

            var password = _walletConfig.Password;
            var accounts = _bolWallet.accounts;

            var codeNameAccount = accounts.Where(account => account.Label == "codename").Single();
            var privateAccount = accounts.Where(account => account.Label == "private").Single();
            var mainAddressAccount = accounts.Where(account => account.Label == "main").Single();

            var blockchainAddressAccount = accounts.Where(account => account.Label == "blockchain").Single();
            var socialAddressAccount = accounts.Where(account => account.Label == "social").Single();
            var votingAddressAccount = accounts.Where(account => account.Label == "voting").Single();
            var commercialAddressAccounts = accounts.Where(account => account.Label == "commercial");

            var codeNameKey = RunInBackground(() => _exportKeyFactory.GetDecryptedPrivateKey(codeNameAccount.Key, password, n, r, p));
            var privateKey = RunInBackground(() => _exportKeyFactory.GetDecryptedPrivateKey(privateAccount.Key, password, n, r, p));
            var blockchainKey = RunInBackground(() => _exportKeyFactory.GetDecryptedPrivateKey(blockchainAddressAccount.Key, password, n, r, p));
            var socialKey = RunInBackground(() => _exportKeyFactory.GetDecryptedPrivateKey(socialAddressAccount.Key, password, n, r, p));
            var votingKey = RunInBackground(() => _exportKeyFactory.GetDecryptedPrivateKey(votingAddressAccount.Key, password, n, r, p));
            var commercialKeys = commercialAddressAccounts
                .Select(account => (account.Address, account.Key))
                .Select(tuple =>
                    (tuple.Address, Task: RunInBackground(() =>
                        _exportKeyFactory.GetDecryptedPrivateKey(tuple.Key, password, n, r, p))))
                .ToArray();

            var allTasks = new List<Task<byte[]>>
            {
                codeNameKey,
                privateKey,
                blockchainKey,
                socialKey,
                votingKey
            };
            allTasks.AddRange(commercialKeys.Select(x => x.Task).ToArray());
            
            var queue = new Queue<Task>(allTasks);
            var maxParallelization = Math.Min(Environment.ProcessorCount, queue.Count);
            var tasks = Enumerable.Range(1, maxParallelization).Select(x => queue.Dequeue()).ToList();

            while (tasks.Count > 0)
            {
                var completedIndex = Task.WaitAny(tasks.ToArray());
                tasks.RemoveAt(completedIndex);
                if (queue.Count > 0)
                {
                    tasks.Add(queue.Dequeue());
                }
            }
            
            var codeNameAccountKeyPair = _keyPairFactory.Create(codeNameKey.Result);
            var privateAccountKeyPair = _keyPairFactory.Create(privateKey.Result);
            var blockChainAddressAccountKeyPair = _keyPairFactory.Create(blockchainKey.Result);
            var socialAddressAccountKeyPair = _keyPairFactory.Create(socialKey.Result);
            var votingAddressAccountKeyPair = _keyPairFactory.Create(votingKey.Result);

            var comm = commercialKeys
                .Select(tuple => (tuple.Address, KeyPair: _keyPairFactory.Create(tuple.Task.Result)))
                .Select(tuple => (ScriptHash: _addressTransformer.ToScriptHash(tuple.Address), tuple.KeyPair))                
                .ToDictionary(tuple => tuple.ScriptHash, tuple => tuple.KeyPair);

            return new BolContext(
                _bolConfig.Contract,
                codeNameAccount.Extra.codename,
                codeNameAccount.Extra.edi,
                codeNameAccountKeyPair,
                privateAccountKeyPair,
                _addressTransformer.ToScriptHash(mainAddressAccount.Address),
                new KeyValuePair<IScriptHash, IKeyPair>(_addressTransformer.ToScriptHash(blockchainAddressAccount.Address), blockChainAddressAccountKeyPair),
                new KeyValuePair<IScriptHash, IKeyPair>(_addressTransformer.ToScriptHash(socialAddressAccount.Address), socialAddressAccountKeyPair),
                new KeyValuePair<IScriptHash, IKeyPair>(_addressTransformer.ToScriptHash(votingAddressAccount.Address), votingAddressAccountKeyPair),
                comm
            );
        }

        public IBolContext GetContext()
        {
            return string.IsNullOrWhiteSpace(_bolWallet.Name) 
                ? new UnauthorizedBolContext(_bolConfig.Contract) 
                : _iCachingService.GetOrCreate(CacheKeyNames.BolContext.ToString(), () => CreateContext());
        }

        private static async Task<T> RunInBackground<T>(Func<T> func) => await Task.Run(func);
    }
}
