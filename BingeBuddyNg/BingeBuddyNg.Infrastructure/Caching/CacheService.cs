using BingeBuddyNg.Core.Infrastructure;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace BingeBuddyNg.Infrastructure
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache memoryCache;

        public CacheService(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> getCallback, TimeSpan expirationTimespan)
        {
            T value = default(T);
            if (memoryCache.TryGetValue<T>(key, out value) == false)
            {
                value = await getCallback();

                memoryCache.Set(key, value, expirationTimespan);
            }

            return value;
        }

        public void Remove(string key)
        {
            memoryCache.Remove(key);
        }
    }
}
