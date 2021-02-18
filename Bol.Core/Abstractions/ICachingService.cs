using System;
using System.Collections.Generic;
using System.Text;
using Bol.Cryptography;

namespace Bol.Core.Abstractions
{
    public interface ICachingService
    {
        T GetOrCreate<T>(object key, Func<T> createItem);
    }
}
