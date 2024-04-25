using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bol.Core.Model;

namespace Bol.Core.Abstractions
{
    public interface IWalletService
    {
        Task<BolWallet> CreateWalletB(string walletPassword, string codeName, string edi, string privateKey = null, CancellationToken token = default);
        Task<BolWallet> CreateWalletC(string walletPassword, string codeName, string edi, string privateKey = null, CancellationToken token = default);
        BolWallet MigrateWallet(string wallet, IEnumerable<string> addresses, string password, string newPassword);
        bool CheckWalletPassword(string wallet, string password);
    }
}
