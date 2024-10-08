using System.Threading;
using System.Threading.Tasks;
using Bol.Core.Model;
using Bol.Core.Transactions;

namespace Bol.Core.Rpc.Abstractions
{
    public interface IRpcMethodFactory
    {
        Task<T> SendRawTransaction<T>(BolTransaction transaction, CancellationToken token = default);
        Task<T> TestRawTransaction<T>(BolTransaction transaction, CancellationToken token = default);
        Task<BolAccount> GetAccount(string codeName, CancellationToken token = default);
        Task<string> GetBolHash(CancellationToken token = default);
    }
}
