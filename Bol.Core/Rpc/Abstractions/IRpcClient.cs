using System.Threading;
using System.Threading.Tasks;

namespace Bol.Core.Rpc.Abstractions
{
    public interface IRpcClient
    {
        Task<T> InvokeAsync<T>(string method, object[] @params, CancellationToken token = default);
    }
}
