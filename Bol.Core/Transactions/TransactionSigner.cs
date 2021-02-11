using System;
using Bol.Cryptography;
using Bol.Cryptography.Abstractions;

namespace Bol.Core.Transactions
{
    public class TransactionSigner : ITransactionSigner
    {
        private readonly ITransactionSerializer _transactionHasher;
        private readonly IECCurveSigner _signer;

        public TransactionSigner(ITransactionSerializer transactionHasher, IECCurveSigner signer)
        {
            _transactionHasher = transactionHasher ?? throw new ArgumentNullException(nameof(transactionHasher));
            _signer = signer ?? throw new ArgumentNullException(nameof(signer));
        }

        public byte[] GenerateSignature(BolTransaction transaction, IKeyPair key)
        {
            var transactionHash = _transactionHasher.SerializeUnsigned(transaction);
            return _signer.Sign(transactionHash, key.PrivateKey, key.PublicKey.ToRawValue());
        }
    }
}
