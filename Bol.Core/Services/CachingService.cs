using System;
using Bol.Core.Abstractions;
using Bol.Cryptography;
using Microsoft.Extensions.Caching.Memory;

namespace Bol.Core.Services
{
    public class KeyPairCachingService : IIKeyPairCachingService
    {
        private MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());        

        public IKeyPair GetOrCreate(object key, Func<IKeyPair> createItem)
        {
            if (!_cache.TryGetValue(key, out IKeyPair cacheEntry))
            {
                cacheEntry = createItem();
                _cache.Set(key, cacheEntry);
            }
            return cacheEntry;
        }
    }
}
