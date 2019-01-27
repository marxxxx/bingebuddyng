using BingeBuddyNg.Shared;

namespace BingeBuddyNg.Services.Infrastructure
{
    public class NotificationMessage
    {
        public string icon { get; set; }
        public string badge { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public NotificationData data { get; set; }

        public NotificationMessage(string icon, string badge, string url, string title, string body)
        {
            this.icon = icon;
            this.badge = badge;
            this.title = title;
            this.body = body;
            this.data = new NotificationData(url);
        }

        public NotificationMessage(string title, string body)
        {
            this.icon = Constants.Urls.ApplicationIconUrl;
            this.badge = Constants.Urls.ApplicationIconUrl;
            this.title = title;
            this.body = body;
            this.data = new NotificationData(Constants.Urls.ApplicationUrl);
        }
    }
}
