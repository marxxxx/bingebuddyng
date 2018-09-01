using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Configuration
{
    public class AppConfiguration
    {
        public string StorageConnectionString { get; set; }
        public string GoogleAPIKey { get; set; }
        public string WebPushPublicKey { get; set; }
        public string WebPushPrivateKey { get; set; }

        public AppConfiguration(string storageConnectionString, string googleApiKey, 
            string webPushPublicKey, string webPushPrivateKey)
        {
            this.StorageConnectionString = storageConnectionString ?? throw new ArgumentNullException(nameof(storageConnectionString));
            this.GoogleAPIKey = googleApiKey ?? throw new ArgumentNullException(nameof(googleApiKey));
            this.WebPushPublicKey = webPushPublicKey ?? throw new ArgumentNullException(nameof(webPushPublicKey));
            this.WebPushPrivateKey = webPushPrivateKey ?? throw new ArgumentNullException(nameof(webPushPrivateKey));
        }

        public override string ToString()
        {
            return $"{{{nameof(StorageConnectionString)}={StorageConnectionString}, {nameof(GoogleAPIKey)}={GoogleAPIKey}, {nameof(WebPushPublicKey)}={WebPushPublicKey}, {nameof(WebPushPrivateKey)}={WebPushPrivateKey}}}";
        }
    }
}
