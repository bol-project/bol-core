using System;
using System.Collections.Generic;
using Bol.Cryptography;

namespace Bol.Address
{
    public interface ISignatureScriptFactory
    {
        ISignatureScript Create(IPublicKey publicKey);
        ISignatureScript Create(IEnumerable<IPublicKey> publicKeys, int numberOfSignatures);
        ISignatureScript Create(byte[] script);
    }
}
