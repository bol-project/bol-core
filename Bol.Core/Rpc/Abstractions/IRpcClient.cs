using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bol.Core.Rpc.Model;

namespace Bol.Core.Rpc.Abstractions
{
    public interface IRpcClient
    {
        Task<T> InvokeAsync<T>(string hexTx, string method, CancellationToken token = default);
    }
}
