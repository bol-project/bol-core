using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bol.Core.Abstractions;
using Neo;
using Neo.Cryptography.ECC;
using Neo.Ledger;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract;
using Neo.VM;
using Neo.Wallets;

namespace Bol.Core.Services
{
    public class ContractService : IContractService
    {
        private readonly ITransactionPublisher _transactionPublisher;
        private readonly IBlockChainService _blockChainService;

        public ContractService(
            ITransactionPublisher transactionPublisher,
            IBlockChainService blockChainService)
        {
            _transactionPublisher = transactionPublisher ?? throw new ArgumentNullException(nameof(transactionPublisher));
            _blockChainService = blockChainService ?? throw new ArgumentNullException(nameof(blockChainService));
        }

        public InvocationTransaction DeployContract(byte[] script, string name, string version, string author, string email, string description, IEnumerable<KeyPair> keys, int numberOfSignatures = 0)
        {
            var multiSig = CreateAddressContract(keys, numberOfSignatures);
            var executionScript = CreateContractScript(script, name, version, author, email, description);
            var transaction = CreateTransaction(executionScript, multiSig);
            var transactionSigned = SignTransaction(transaction, multiSig, keys);

            if (!transactionSigned) throw new Exception("Could not sign transaction.");

            SubmitTransaction(transaction);

            return transaction;
        }

        public InvocationTransaction InvokeContract(string contract, string operation, IEnumerable<byte[]> parameters, string description = null, IEnumerable<string> remarks = null, IEnumerable<KeyPair> keys = null, int numberOfSignatures = 0)
        {
            var transaction = CreateTransaction(contract, operation, parameters, description, remarks, keys, numberOfSignatures);
            InvokeContract(transaction);
            return transaction;
        }

        public void InvokeContract(InvocationTransaction transaction)
        {
            SubmitTransaction(transaction);
        }

        public ContractExecutionResult TestContract(string contract, string operation, IEnumerable<byte[]> parameters, string description = null, IEnumerable<string> remarks = null, IEnumerable<KeyPair> keys = null, int numberOfSignatures = 0)
        {
            var transaction = CreateTransaction(contract, operation, parameters, description, remarks, keys, numberOfSignatures);

            return TestContract(transaction);
        }

        public ContractExecutionResult TestContract(InvocationTransaction transaction)
        {
            var engine = ApplicationEngine.Run(transaction.Script, transaction);

            if (engine.State.HasFlag(VMState.FAULT)) return ContractExecutionResult.Fail();

            return ContractExecutionResult.Succeed(engine.ResultStack.First().GetByteArray(), engine.GasConsumed);
        }

        public InvocationTransaction CreateTransaction(string contract, string operation, IEnumerable<byte[]> parameters, string description = null, IEnumerable<string> remarks = null, IEnumerable<KeyPair> keys = null, int numberOfSignatures = 0)
        {
            var scriptHash = ParseOrThrowIfNotFound(contract);
            var executionScript = CreateExecutionScript(scriptHash, operation, parameters);

            var addressContract = CreateAddressContract(keys, numberOfSignatures);

            InvocationTransaction transaction;
            if (keys != null && keys.Any())
            {
                transaction = CreateTransaction(executionScript, addressContract, description, remarks);
                var transactionSigned = SignTransaction(transaction, addressContract, keys);
                if (!transactionSigned) throw new Exception("Could not sign transaction.");
            }
            else
            {
                transaction = CreateTransaction(executionScript, description: description, remarks: remarks);
            }

            return transaction;
        }

        private UInt160 ParseOrThrowIfNotFound(string contract)
        {
            var result = _blockChainService.GetContract(contract);

            if (!result.ContractExists)
            {
                throw new Exception("Contract was not found in the blockchain.");
            }

            return result.ScriptHash;
        }

        private Contract CreateAddressContract(IEnumerable<KeyPair> keys, int numberOfSignatures)
        {
            var publicKeys = keys
                .Select(key => key.PublicKey)
                .ToArray();

            if (publicKeys.Length == 1)
            {
                return Contract.CreateSignatureContract(publicKeys[0]);
            }

            if (numberOfSignatures < publicKeys.Length || numberOfSignatures > publicKeys.Length)
            {
                throw new Exception("Number of signatures must be in the range of 1 -> Number of Keys");
            }

            var multisig = Contract.CreateMultiSigContract(numberOfSignatures, publicKeys);
            return multisig;
        }

        private byte[] CreateExecutionScript(UInt160 scriptHash, string operation, IEnumerable<byte[]> parameters)
        {
            var contractParameters = parameters
                .Select(parameter => new ContractParameter
                {
                    Type = ContractParameterType.ByteArray,
                    Value = parameter
                })
                .ToArray();

            var invocationParameters = new[]
{
                new ContractParameter
                {
                    Type = ContractParameterType.String,
                    Value = operation
                },
                new ContractParameter
                {
                    Type = ContractParameterType.Array,
                    Value = contractParameters
                }
            };

            using (var sb = new ScriptBuilder())
            {
                sb.EmitAppCall(scriptHash, invocationParameters);
                return sb.ToArray();
            }
        }

        private byte[] CreateContractScript(byte[] script, string name, string version, string author, string email, string description)
        {
            byte[] parameter_list = "0710".HexToBytes();
            var return_type = ContractParameterType.ByteArray;
            var properties = ContractPropertyState.HasStorage;

            using (var sb = new ScriptBuilder())
            {
                sb.EmitSysCall("Neo.Contract.Create", script, parameter_list, return_type, properties, name, version, author, email, description);
                return sb.ToArray();
            }
        }

        private InvocationTransaction CreateTransaction(byte[] executionScript, Contract contract = null, string description = null, IEnumerable<string> remarks = null)
        {
            var transaction = new InvocationTransaction
            {
                Version = 1,
                Script = executionScript,
                Gas = Fixed8.Zero,
                Inputs = new CoinReference[0],
                Outputs = new TransactionOutput[0],
                Witnesses = new Witness[0]
            };
            var attributes = new List<TransactionAttribute>();

            if (contract != null)
            {
                attributes.Add(
                    new TransactionAttribute
                    {
                        Usage = TransactionAttributeUsage.Script,
                        Data = contract.ScriptHash.ToArray()
                    });
            }

            if (description != null)
            {
                attributes.Add(
                    new TransactionAttribute
                    {
                        Usage = TransactionAttributeUsage.Description,
                        Data = Encoding.UTF8.GetBytes(description),
                    });
            }

            if (remarks != null && remarks.Any())
            {
                var remarkAttributes = Enumerable
                    .Range(0xf0, 0xff)
                    .Zip(remarks, (index, remark) => new TransactionAttribute
                    {
                        Usage = (TransactionAttributeUsage)index,
                        Data = Encoding.UTF8.GetBytes(remark),
                    });

                attributes.AddRange(remarkAttributes);
            }

            transaction.Attributes = attributes.ToArray();

            return transaction;
        }

        private bool SignTransaction(InvocationTransaction transaction, Contract multiSig, IEnumerable<KeyPair> keys)
        {
            var context = new ContractParametersContext(transaction);
            foreach (var key in keys)
            {
                context.AddSignature(multiSig, key.PublicKey, transaction.Sign(key));
            }

            if (!context.Completed) return false;

            transaction.Witnesses = context.GetWitnesses();
            return true;
        }

        private void SubmitTransaction(InvocationTransaction transaction)
        {
            _transactionPublisher.Publish(transaction);
        }

        public Contract CreateMultiSigContract(int requiredSignatures, params ECPoint[] publicKeys)
        {
            return Contract.CreateMultiSigContract(requiredSignatures, publicKeys);
        }

        public Contract CreateSignatureContract(ECPoint publicKey)
        {
            return Contract.CreateSignatureContract(publicKey);
        }
    }

    public class ContractExecutionResult
    {
        public bool Success { get; private set; }
        public Fixed8 GasConsumed { get; private set; }
        public byte[] Result { get; private set; }

        public static ContractExecutionResult Succeed(byte[] result, Fixed8 gas = default)
        {
            return new ContractExecutionResult
            {
                Success = true,
                GasConsumed = gas,
                Result = result,
            };
        }

        public static ContractExecutionResult Fail()
        {
            return new ContractExecutionResult
            {
                Success = false
            };
        }
    }
}
