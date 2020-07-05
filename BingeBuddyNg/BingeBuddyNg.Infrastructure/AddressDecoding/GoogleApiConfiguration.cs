using System;

namespace BingeBuddyNg.Infrastructure
{
    public class GoogleApiConfiguration
    {
        public GoogleApiConfiguration(string apiKey)
        {
            ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        public string ApiKey { get; }
    }
}
