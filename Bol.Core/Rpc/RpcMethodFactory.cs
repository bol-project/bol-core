using System;
using System.Threading;
using System.Threading.Tasks;
using Bol.Core.Rpc.Abstractions;
using Bol.Core.Transactions;
using Bol.Cryptography;

namespace Bol.Core.Rpc
{
    public class RpcMethodFactory : IRpcMethodFactory
    {
        private readonly IRpcClient _rpcClient;
        private readonly ITransactionSerializer _transactionSerializer;
        private readonly IBase16Encoder _bse16Encoder;

        public RpcMethodFactory(IRpcClient rpcClient, ITransactionSerializer transactionSerializer, IBase16Encoder bse16Encoder)
        {
            _rpcClient = rpcClient ?? throw new ArgumentNullException(nameof(rpcClient));
            _transactionSerializer = transactionSerializer ?? throw new ArgumentNullException(nameof(transactionSerializer));
            _bse16Encoder = bse16Encoder ?? throw new ArgumentNullException(nameof(bse16Encoder));
        }

        public Task<T> SendRawTransaction<T>(BolTransaction bolTx, CancellationToken token = default)
        {
            var bolSignedTransaction = _transactionSerializer.SerializeSigned(bolTx);

            return _rpcClient.InvokeAsync<T>("sendrawtransaction", new[] { _bse16Encoder.Encode(bolSignedTransaction) }, token);
        }

        public Task<T> TestRawTransaction<T>(BolTransaction transaction, CancellationToken token = default)
        {
            var clone = new BolTransaction
            {
                Attributes = transaction.Attributes,
                ExecutionScript = transaction.ExecutionScript                
            };
            var unsignedTransaction = _transactionSerializer.SerializeSigned(clone);

            return _rpcClient.InvokeAsync<T>("testrawtransaction", new[] { _bse16Encoder.Encode(unsignedTransaction) }, token);
        }

        public Task<T> GetAccount<T>(string codeName, CancellationToken token = default)
        {
            return _rpcClient.InvokeAsync<T>("getaccount", new[] { codeName }, token);
        }
    }
}
