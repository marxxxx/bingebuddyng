using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Configuration
{
    public class AppConfiguration
    {
        public string StorageConnectionString { get; set; }
        public string GoogleAPIKey { get; set; }

        public AppConfiguration(string storageConnectionString, string googleApiKey)
        {
            this.StorageConnectionString = storageConnectionString ?? throw new ArgumentNullException(nameof(storageConnectionString));
            this.GoogleAPIKey = googleApiKey ?? throw new ArgumentNullException(nameof(googleApiKey));
        }
    }
}
