namespace BingeBuddyNg.Core.Infrastructure
{
    public class WebPushNotificationData
    {
        public WebPushNotificationData(string url)
        {
            this.url = url;
        }

        public string url { get; set; }
    }
}
