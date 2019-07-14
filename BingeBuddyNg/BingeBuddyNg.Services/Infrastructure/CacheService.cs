using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Infrastructure
{
    public class CacheService : ICacheService
    {
        public CacheService(IMemoryCache memoryCache)
        {
            MemoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public IMemoryCache MemoryCache { get; }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> getCallback, TimeSpan expirationTimespan)
        {
            T value = default(T);
            if(MemoryCache.TryGetValue<T>(key, out value) == false)
            {
                value = await getCallback();

                MemoryCache.Set(key, value, expirationTimespan);
            }

            return value;
        }

        public void Remove(string key)
        {
            MemoryCache.Remove(key);
        }
    }
}
