using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Infrastructure
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
