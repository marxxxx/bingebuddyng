using System;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Infrastructure
{
    public class NoCacheService : ICacheService
    {
        public Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> getCallback, TimeSpan expirationTimespan)
        {
            return getCallback();
        }

        public void Remove(string key)
        {
        }
    }
}
