using System;
using System.Linq;
using Bol.Cryptography.Neo.Core.ECC;

namespace Bol.Cryptography.Neo.Keys
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

        public byte[] ToRawValue()
        {
            return ECPoint.EncodePoint(false).Skip(1).ToArray();
        }
    }
}
