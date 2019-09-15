using System;
using System.Collections.Generic;
using System.Text;
using Neo.Cryptography.ECC;
using Neo.SmartContract;

namespace Bol.Core.Abstractions
{
    public interface IContractService
    {
        Contract CreateSignatureContract(ECPoint publicKey);
        Contract CreateMultiSigContract(int requiredSignatures, params ECPoint[] publicKeys);
    }
}
