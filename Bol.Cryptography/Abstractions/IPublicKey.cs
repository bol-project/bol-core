using System;

namespace Bol.Cryptography
{
    public interface IPublicKey : IComparable<IPublicKey>
    {
        byte[] ToBytes();
        byte[] ToRawValue();
    }
}
