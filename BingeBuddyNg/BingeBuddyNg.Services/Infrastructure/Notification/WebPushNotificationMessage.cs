using BingeBuddyNg.Shared;

namespace BingeBuddyNg.Core.Infrastructure
{
    public class WebPushNotificationMessage
    {
        public string icon { get; set; }
        public string badge { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public WebPushNotificationData data { get; set; }

        public WebPushNotificationMessage(string icon, string badge, string url, string title, string body)
        {
            this.icon = icon;
            this.badge = badge;
            this.title = title;
            this.body = body;
            this.data = new WebPushNotificationData(url);
        }

        public WebPushNotificationMessage(string title, string body, string url = Constants.Urls.ApplicationUrl)
        {
            this.icon = Constants.Urls.ApplicationIconUrl;
            this.badge = Constants.Urls.ApplicationIconUrl;
            this.title = title;
            this.body = body;
            this.data = new WebPushNotificationData(url);
        }
    }
}
