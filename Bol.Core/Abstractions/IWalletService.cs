using System.Threading;
using System.Threading.Tasks;
using Bol.Core.Model.Wallet;

namespace Bol.Core.Abstractions
{
    public interface IWalletService
    {
        Task<BolWallet> CreateWallet(string walletPassword, string codeName, string edi, string privateKey = null, CancellationToken token = default);
    }
}
