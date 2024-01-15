using System;
using System.Linq;
using System.Numerics;
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

            // Reverse the byte array to ensure big-endian interpretation and add zero byte sign
            var bytes = privateKey
                .Reverse()
                .Concat(new byte[] { 0x00 })
                .ToArray();

            // Create BigInteger from the modified byte array
            var privateKeyBigInteger = new BigInteger(bytes);

            if (privateKeyBigInteger >= ECCurve.Secp256r1.N)
            {
                throw new KeyPairException("Private key cannot be higher than ECCurve N value.");
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

    public class KeyPairException : Exception
    {
        public KeyPairException(string message) : base(message) { }
    }
}
