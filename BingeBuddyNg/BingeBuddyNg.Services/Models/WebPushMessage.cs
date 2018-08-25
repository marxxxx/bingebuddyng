using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Models
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
