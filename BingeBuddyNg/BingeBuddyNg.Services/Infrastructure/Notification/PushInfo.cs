using System;

namespace BingeBuddyNg.Core.Infrastructure
{
    public class PushInfo
    {
        public PushInfo()
        {
        }

        public PushInfo(string subscriptionEndpoint, string auth, string p256dh)
        {
            SubscriptionEndpoint = subscriptionEndpoint ?? throw new ArgumentNullException(nameof(subscriptionEndpoint));
            Auth = auth ?? throw new ArgumentNullException(nameof(auth));
            this.p256dh = p256dh ?? throw new ArgumentNullException(nameof(p256dh));
        }

        public string SubscriptionEndpoint { get; set; }
        public string Auth { get; set; }
        public string p256dh { get; set; }

        public bool HasValue()
        {
            return !string.IsNullOrEmpty(SubscriptionEndpoint) && !string.IsNullOrEmpty(Auth) && !string.IsNullOrEmpty(p256dh);
        }
    }
}
