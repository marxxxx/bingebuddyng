using System;

namespace BingeBuddyNg.Functions.Services.Notifications
{
    public abstract class NotificationBase
    {
        public string UserId { get; }

        public string Url { get; }

        protected NotificationBase(string userId)
        {
            this.UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        }

        protected NotificationBase(string userId, string url)
        {
            this.UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            this.Url = url ?? throw new ArgumentNullException(nameof(url));
        }
    }
}