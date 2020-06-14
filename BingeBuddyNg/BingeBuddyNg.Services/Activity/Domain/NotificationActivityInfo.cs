using System;

namespace BingeBuddyNg.Services.Activity.Domain
{
    public class NotificationActivityInfo
    {
        public string Message { get; private set; }

        public NotificationActivityInfo(string message)
        {
            this.Message = message;
        }
    }
}
