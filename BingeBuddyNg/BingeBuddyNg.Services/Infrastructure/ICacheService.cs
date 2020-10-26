using System;
using System.Threading.Tasks;

namespace BingeBuddyNg.Core.Infrastructure
{
    public interface ICacheService
    {
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> getCallback, TimeSpan expirationTimespan);

        void Remove(string key);
    }
}