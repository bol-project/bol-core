using System;

namespace Bol.Core.Abstractions
{
    public interface ICachingService
    {
        T GetOrCreate<T>(object key, Func<T> createItem);
    }
}
