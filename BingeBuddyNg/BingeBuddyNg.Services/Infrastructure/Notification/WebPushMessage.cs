using System;

namespace BingeBuddyNg.Services.Infrastructure
{
    public class WebPushMessage
    {
        public NotificationMessage notification { get; set; }

        public WebPushMessage(NotificationMessage notification)
        {
            this.notification = notification ?? throw new ArgumentNullException(nameof(notification));
        }
    }
}
