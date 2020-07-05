using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Infrastructure
{
    public class WebPushConfiguration
    {
        public WebPushConfiguration(string webPushPublicKey, string webPushPrivateKey)
        {
            WebPushPublicKey = webPushPublicKey ?? throw new ArgumentNullException(nameof(webPushPublicKey));
            WebPushPrivateKey = webPushPrivateKey ?? throw new ArgumentNullException(nameof(webPushPrivateKey));
        }

        public string WebPushPublicKey { get; }
        public string WebPushPrivateKey { get; }
    }
}
