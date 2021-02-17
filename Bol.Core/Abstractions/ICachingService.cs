using System;
using System.Collections.Generic;
using System.Text;
using Bol.Cryptography;

namespace Bol.Core.Abstractions
{
    public interface IIKeyPairCachingService
    {
        IKeyPair GetOrCreate(object key, Func<IKeyPair> createItem);
    }
}
