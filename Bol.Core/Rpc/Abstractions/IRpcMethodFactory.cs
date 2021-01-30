using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Bol.Core.Transactions;

namespace Bol.Core.Rpc.Abstractions
{
   public interface IRpcMethodFactory
    {
        Task<T> SendRawTransaction<T>(BolTransaction bolTx, CancellationToken token = default);

        Task<T> GetAccount<T>(string mainAddress, CancellationToken token = default);
    }
}
