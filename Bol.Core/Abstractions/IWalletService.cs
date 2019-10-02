using System.Threading;
using System.Threading.Tasks;
using Neo.Wallets.NEP6;

namespace Bol.Core.Abstractions
{
    public interface IWalletService
    {
        Task<NEP6Wallet> CreateWallet(string codeName, byte[] privateKey, string walletPassword, CancellationToken token = default);
    }
}