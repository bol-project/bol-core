using System.Threading;
using System.Threading.Tasks;
using Neo.Wallets.NEP6;

namespace Bol.Core.Abstractions
{
    public interface IWalletService
    {
        Task<string> CreateWallet(string walletPassword, string codeName, string edi, string privateKey = null, CancellationToken token = default);
    }
}
