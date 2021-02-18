using System;
using Bol.Core.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace Bol.Core.Services
{
    public class CachingService : ICachingService
    {   
        private readonly IMemoryCache _memoryCache;
        public CachingService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public T GetOrCreate<T>(object key, Func<T> createItem)
        {
            if (!_memoryCache.TryGetValue(key, out T cacheEntry))
            {
                cacheEntry = createItem();
                _memoryCache.Set(key, cacheEntry);
            }
            return cacheEntry;
        }
    }
}
