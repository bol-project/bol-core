using System;
using Neo.Cryptography.ECC;

namespace Bol.Cryptography.Keys
{
    public class PublicKey : IPublicKey
    {
        public PublicKey(ECPoint ecPoint)
        {
            ECPoint = ecPoint ?? throw new ArgumentNullException(nameof(ecPoint));
        }

        public ECPoint ECPoint { get; private set; }

        public int CompareTo(IPublicKey other)
        {
            return ECPoint.CompareTo(((PublicKey)other).ECPoint);
        }

        public byte[] ToBytes()
        {
            return ECPoint.EncodePoint(true);
        }
    }
}
