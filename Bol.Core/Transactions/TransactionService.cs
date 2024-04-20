using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bol.Address;
using Bol.Core.Rpc.Abstractions;
using Bol.Cryptography;

namespace Bol.Core.Transactions
{
    public class TransactionService : ITransactionService
    {
        private readonly ISignatureScriptFactory _signatureScriptFactory;
        private readonly IScriptHashFactory _scriptHashFactory;
        private readonly ITransactionNotarizer _transactionNotarizer;
        private readonly IRpcMethodFactory _rpc;

        public TransactionService(ISignatureScriptFactory signatureScriptFactory, IScriptHashFactory scriptHashFactory, ITransactionNotarizer transactionNotarizer, IRpcMethodFactory rpc)
        {
            _signatureScriptFactory = signatureScriptFactory ?? throw new ArgumentNullException(nameof(signatureScriptFactory));
            _scriptHashFactory = scriptHashFactory ?? throw new ArgumentNullException(nameof(scriptHashFactory));
            _transactionNotarizer = transactionNotarizer ?? throw new ArgumentNullException(nameof(transactionNotarizer));
            _rpc = rpc ?? throw new ArgumentNullException(nameof(rpc));
        }

        public BolTransaction Create(
            ISignatureScript witness,
            string contract,
            string operation,
            byte[][] parameters,
            string description = null,
            IEnumerable<string> remarks = null)
        {
            //TODO: Add validations

            var contractHash = _scriptHashFactory.Create(contract);
            var executionScript = _signatureScriptFactory.CreateContractOperationScript(contractHash, operation, parameters);

            var transaction = CreateBolTransaction(executionScript, witness?.ToScriptHash(), description, remarks);

            return transaction;
        }

        public BolTransaction Create(
            ISignatureScript witness,
            byte[] script,
            string description = null,
            IEnumerable<string> remarks = null)
        {
            //TODO: Add validations

            var executionScript = _signatureScriptFactory.Create(script);
            var transaction = CreateBolTransaction(executionScript, witness?.ToScriptHash(), description, remarks);

            return transaction;
        }

        public Task Publish(BolTransaction transaction, CancellationToken token = default)
        {
            return _rpc.SendRawTransaction<bool>(transaction, token);
        }

        public BolTransaction Sign(BolTransaction transaction, ISignatureScript witness, IEnumerable<IKeyPair> keys)
        {
            return _transactionNotarizer.Notarize(transaction, witness, keys);
        }

        public Task<T> Test<T>(BolTransaction transaction, CancellationToken token = default)
        {
            return _rpc.TestRawTransaction<T>(transaction, token);
        }

        private BolTransaction CreateBolTransaction(ISignatureScript executionScript, IScriptHash address = null, string description = null, IEnumerable<string> remarks = null)
        {
            var attributes = new List<BolTransactionAttribute>();

            if (address != null)
            {
                attributes.Add(
                    new BolTransactionAttribute
                    {
                        Type = TransactionAttributeType.Script,
                        Value = address.GetBytes()
                    });
            }

            if (description != null)
            {
                attributes.Add(
                    new BolTransactionAttribute
                    {
                        Type = TransactionAttributeType.Description,
                        Value = Encoding.UTF8.GetBytes(description),
                    });
            }

            if (remarks != null && remarks.Any())
            {
                var remarkAttributes = Enumerable
                    .Range(0xf0, 0xff)
                    .Zip(remarks, (index, remark) => new BolTransactionAttribute
                    {
                        Type = (TransactionAttributeType)index,
                        Value = Encoding.UTF8.GetBytes(remark),
                    });

                attributes.AddRange(remarkAttributes);
            }

            return new BolTransaction
            {
                ExecutionScript = executionScript,
                Attributes = attributes
            };
        }

    }
}
