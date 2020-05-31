using System;

namespace BingeBuddyNg.Services.Infrastructure
{
    public class WebPushMessage
    {
        public WebPushNotificationMessage notification { get; set; }

        public WebPushMessage(WebPushNotificationMessage notification)
        {
            this.notification = notification ?? throw new ArgumentNullException(nameof(notification));
        }
    }
}
