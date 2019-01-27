namespace BingeBuddyNg.Services.Infrastructure
{
    public class NotificationData
    {
        public NotificationData(string url)
        {
            this.url = url;
        }

        public string url { get; set; }
    }
}
