using Bol.Core.Services;
using Neo.Cryptography.ECC;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract;
using Neo.Wallets;
using System.Collections.Generic;

namespace Bol.Core.Abstractions
{
    public interface IContractService
    {
        InvocationTransaction DeployContract(byte[] script, string name, string version, string author, string email, string description, IEnumerable<Cryptography.IKeyPair> keys, int numberOfSignatures = 0);
        InvocationTransaction InvokeContract(string contract, string operation, IEnumerable<byte[]> parameters, string description = null, IEnumerable<string> remarks = null, IEnumerable<Cryptography.IKeyPair> keys = null, int numberOfSignatures = 0);
        void InvokeContract(InvocationTransaction transaction);
        ContractExecutionResult TestContract(string contract, string operation, IEnumerable<byte[]> parameters, string description = null, IEnumerable<string> remarks = null, IEnumerable<Cryptography.IKeyPair> keys = null, int numberOfSignatures = 0);
        ContractExecutionResult TestContract(InvocationTransaction transaction);
        InvocationTransaction CreateTransaction(string contract, string operation, IEnumerable<byte[]> parameters, string description = null, IEnumerable<string> remarks = null, IEnumerable<Cryptography.IKeyPair> keys = null, int numberOfSignatures = 0);
        Contract CreateSignatureContract(ECPoint publicKey);
        Contract CreateMultiSigContract(int requiredSignatures, params ECPoint[] publicKeys);
    }
}
