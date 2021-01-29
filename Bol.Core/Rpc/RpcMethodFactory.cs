using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Bol.Core.Rpc.Abstractions;
using Bol.Core.Transactions;
using Bol.Cryptography;

namespace Bol.Core.Rpc
{
    public class RpcMethodFactory : IRpcMethodFactory
    {

        private IRpcClient _rpcClient;
        private ITransactionSerializer _transactionSerializer;
        private IBase16Encoder _bse16Encoder;

        public RpcMethodFactory(IRpcClient rpcClient, ITransactionSerializer transactionSerializer, IBase16Encoder bse16Encoder)
        {
            _rpcClient = rpcClient ?? throw new ArgumentNullException(nameof(rpcClient));
            _transactionSerializer = transactionSerializer ?? throw new ArgumentNullException(nameof(transactionSerializer));
            _bse16Encoder = bse16Encoder ?? throw new ArgumentNullException(nameof(bse16Encoder));
        }
        public async Task<T> SendRawTransaction<T>(BolTransaction bolTx, CancellationToken token = default) 
        {
            var bolSignedTransaction = _transactionSerializer.SerializeSigned(bolTx); 

           return await _rpcClient.InvokeAsync<T>(_bse16Encoder.Encode(bolSignedTransaction), "sendrawtransaction",  token );
        }

        public async Task<T> GetAccount<T>(string codeName, CancellationToken token = default) 
        {
            return await _rpcClient.InvokeAsync<T>(codeName, "getaccountstate", token);
        }
        
    }
}
