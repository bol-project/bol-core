using System;
using Bol.Cryptography.Keys;
using Bol.Cryptography.Neo.Core.ECC;

namespace Bol.Cryptography.Neo.Keys
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

        public IKeyPair Create()
        {
            var privateKey = new byte[32];
            
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(privateKey);
            }

            return Create(privateKey);
        }
    }
}
