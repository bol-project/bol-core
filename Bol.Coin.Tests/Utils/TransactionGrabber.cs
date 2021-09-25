using System;
using System.Threading;
using System.Threading.Tasks;
using Bol.Core.Rpc.Abstractions;
using Bol.Core.Transactions;

namespace Bol.Coin.Tests.Utils
{
    public class TransactionGrabber
    {
        public BolTransaction Transaction { get; set; }
    }

    public class FakeRpcMethodFactory : IRpcMethodFactory
    {
        private TransactionGrabber _grabber;

        public FakeRpcMethodFactory(TransactionGrabber grabber)
        {
            _grabber = grabber ?? throw new ArgumentNullException(nameof(grabber));
        }

        public Task<T> GetAccount<T>(string mainAddress, CancellationToken token = default)
        {
            return Task.FromResult(default(T));
        }

        public Task<T> SendRawTransaction<T>(BolTransaction transaction, CancellationToken token = default)
        {
            _grabber.Transaction = transaction;
            return Task.FromResult(default(T));
        }

        public Task<T> TestRawTransaction<T>(BolTransaction transaction, CancellationToken token = default)
        {
            _grabber.Transaction = transaction;
            return Task.FromResult(default(T));
        }
    }
}
