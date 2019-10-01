using Akka.Actor;
using Neo;
using Neo.Ledger;
using Neo.Network.P2P;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract;
using Neo.VM;
using Neo.Wallets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bol.Core.Services
{
    public interface IContractService
    {
        InvocationTransaction DeployContract(byte[] script, string name, string version, string author, string email, string description, IEnumerable<KeyPair> keys);
        InvocationTransaction InvokeContract(string contract, string operation, IEnumerable<byte[]> parameters, IEnumerable<KeyPair> keys);
        ContractExecutionResult TestContract(string contract, string operation, IEnumerable<byte[]> parameters, IEnumerable<KeyPair> keys);
    }

    public class ContractService : IContractService
    {
        private readonly IActorRef _localNode;

        public ContractService(IActorRef localNode)
        {
            _localNode = localNode ?? throw new ArgumentNullException(nameof(localNode));
        }

        public InvocationTransaction DeployContract(byte[] script, string name, string version, string author, string email, string description, IEnumerable<KeyPair> keys)
        {
            var multiSig = CreateMultiSig(keys);
            var executionScript = CreateContractScript(script, name, version, author, email, description);
            var transaction = CreateTransaction(executionScript, multiSig);
            var transactionSigned = SignTransaction(transaction, multiSig, keys);

            if (!transactionSigned) throw new Exception("Could not sign transaction.");

            SubmitTransaction(transaction);

            return transaction;
        }

        public InvocationTransaction InvokeContract(string contract, string operation, IEnumerable<byte[]> parameters, IEnumerable<KeyPair> keys)
        {
            var scriptHash = ParseOrThrowIfNotFound(contract);
            var multiSig = CreateMultiSig(keys);
            var executionScript = CreateExecutionScript(scriptHash, operation, parameters);
            var transaction = CreateTransaction(executionScript, multiSig);
            var transactionSigned = SignTransaction(transaction, multiSig, keys);

            if (!transactionSigned) throw new Exception("Could not sign transaction.");

            SubmitTransaction(transaction);

            return transaction;
        }

        public ContractExecutionResult TestContract(string contract, string operation, IEnumerable<byte[]> parameters, IEnumerable<KeyPair> keys)
        {
            var scriptHash = ParseOrThrowIfNotFound(contract);
            var executionScript = CreateExecutionScript(scriptHash, operation, parameters);
            var transaction = CreateTransaction(executionScript);

            ApplicationEngine engine = ApplicationEngine.Run(transaction.Script, transaction);

            if (engine.State.HasFlag(VMState.FAULT)) return ContractExecutionResult.Fail();

            return ContractExecutionResult.Succeed(engine.ResultStack.First().GetByteArray(), engine.GasConsumed);
        }

        private UInt160 ParseOrThrowIfNotFound(string contract)
        {
            var scriptHash = UInt160.Parse(contract);
            ContractState contractState = Blockchain.Singleton.Store.GetContracts().TryGet(scriptHash);

            if (contractState == null)
            {
                throw new Exception("Contract was not found in the blockchain.");
            }

            return scriptHash;
        }

        private Contract CreateMultiSig(IEnumerable<KeyPair> keys)
        {
            var publicKeys = keys
                .Select(key => key.PublicKey)
                .ToArray();
            var multisig = Contract.CreateMultiSigContract(publicKeys.Length, publicKeys);
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

            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitAppCall(scriptHash, invocationParameters);
                return sb.ToArray();
            }
        }

        private byte[] CreateContractScript(byte[] script, string name, string version, string author, string email, string description)
        {
            byte[] parameter_list = "0710".HexToBytes();
            ContractParameterType return_type = ContractParameterType.ByteArray;
            ContractPropertyState properties = ContractPropertyState.HasStorage;

            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitSysCall("Neo.Contract.Create", script, parameter_list, return_type, properties, name, version, author, email, description);
                return sb.ToArray();
            }
        }

        private InvocationTransaction CreateTransaction(byte[] executionScript, Contract multiSig = null)
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

            if (multiSig != null)
            {
                transaction.Attributes = new[]
                {
                    new TransactionAttribute {
                        Usage = TransactionAttributeUsage.Script,
                        Data = multiSig.ScriptHash.ToArray()
                    }
                };
            }

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
            _localNode.Tell(new LocalNode.Relay { Inventory = transaction });
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
