using System;
using System.Collections.Generic;
using System.Text;
using Neo.Cryptography.ECC;

namespace Bol.Cryptography.Keys
{
    public class KeyPairFactory : IKeyPairFactory
    {
        public IKeyPair Create(byte[] privateKey)
        {
            if (privateKey == null || privateKey.Length != 32)
            {
                throw new ArgumentException("Bad private key format.");
            }

            var publicKey = new PublicKey(ECCurve.Secp256r1.G * privateKey);
            return new KeyPair(privateKey, publicKey);
        }
    }
}
