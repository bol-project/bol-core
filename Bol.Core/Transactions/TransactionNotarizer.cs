using System;
using System.Collections.Generic;
using System.Linq;
using Bol.Address;
using Bol.Cryptography;

namespace Bol.Core.Transactions
{
    public class TransactionNotarizer : ITransactionNotarizer
    {
        private readonly ITransactionSigner _signer;

        public TransactionNotarizer(ITransactionSigner signer)
        {
            _signer = signer ?? throw new ArgumentNullException(nameof(signer));
        }

        public BolTransaction Notarize(BolTransaction transaction, ISignatureScript witness, IEnumerable<IKeyPair> keys)
        {
            var signatures = keys.OrderBy(key => key.PublicKey)
                .Select(key => _signer.GenerateSignature(transaction, key))
                .Select(signature => new byte[] { 0x40 }.Concat(signature).ToArray())
                .Aggregate( (s1,s2) => s1.Concat(s2).ToArray());

            //using var sb = new ScriptBuilder();
            //foreach (var signature in signatures)
            //{
            //    sb.EmitPush(signature);
            //}

            transaction.Witnesses = new[]
            {
                new BolTransactionWitness
                {
                    InvocationScript = signatures,
                    VerificationScript = witness.GetBytes()
                }
            };
            return transaction;
        }
    }
}
