using Bol.Core.Abstractions;
using Neo.Cryptography.ECC;
using Neo.SmartContract;

namespace Bol.Core.Services
{
    public class ContractService : IContractService
    {
        public Contract CreateMultiSigContract(int requiredSignatures, params ECPoint[] publicKeys)
        {
            return Contract.CreateMultiSigContract(requiredSignatures, publicKeys);
        }

        public Contract CreateSignatureContract(ECPoint publicKey)
        {
            return Contract.CreateSignatureContract(publicKey);
        }
    }
}
