namespace BingeBuddyNg.Services.Infrastructure
{
    public class PushInfo
    {
        public string SubscriptionEndpoint { get; set; }
        public string Auth { get; set; }
        public string p256dh { get; set; }

        public bool HasValue()
        {
            return !string.IsNullOrEmpty(SubscriptionEndpoint) && !string.IsNullOrEmpty(Auth) && !string.IsNullOrEmpty(p256dh);
        }
    }
}
