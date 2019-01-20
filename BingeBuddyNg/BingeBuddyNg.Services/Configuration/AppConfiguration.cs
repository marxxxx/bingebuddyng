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
        public string FourSquareApiClientKey { get; set; }
        public string FourSquareApiClientSecret { get; set; }

        public AppConfiguration(string storageConnectionString, string googleApiKey, 
            string webPushPublicKey, string webPushPrivateKey,
            string fourSquareApiClientKey, string fourSquareApiClientSecret)
        {
            this.StorageConnectionString = storageConnectionString ?? throw new ArgumentNullException(nameof(storageConnectionString));
            this.GoogleAPIKey = googleApiKey ?? throw new ArgumentNullException(nameof(googleApiKey));
            this.WebPushPublicKey = webPushPublicKey ?? throw new ArgumentNullException(nameof(webPushPublicKey));
            this.WebPushPrivateKey = webPushPrivateKey ?? throw new ArgumentNullException(nameof(webPushPrivateKey));
            this.FourSquareApiClientKey = fourSquareApiClientKey ?? throw new ArgumentNullException(nameof(fourSquareApiClientKey));
            this.FourSquareApiClientSecret = fourSquareApiClientSecret ?? throw new ArgumentNullException(nameof(fourSquareApiClientSecret));
        }

        public override string ToString()
        {
            return $"{{{nameof(StorageConnectionString)}={StorageConnectionString}, {nameof(GoogleAPIKey)}={GoogleAPIKey}, {nameof(WebPushPublicKey)}={WebPushPublicKey}, {nameof(WebPushPrivateKey)}={WebPushPrivateKey}}}";
        }
    }
}
