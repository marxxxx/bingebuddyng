using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Models
{
    public class NotificationMessage
    {
        public string icon { get; set; }
        public string title { get; set; }
        public string body { get; set; }

        public NotificationMessage(string icon, string title, string body)
        {
            this.icon = icon;
            this.title = title;
            this.body = body;
        }
    }
}
