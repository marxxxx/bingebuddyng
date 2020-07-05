using System;

namespace BingeBuddyNg.Core.Infrastructure
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
