using System;

namespace BingeBuddyNg.Services.Activity.Domain
{
    public class NotificationActivity : Activity
    {
        public string Message { get; private set; }

        private NotificationActivity(string id, ActivityType type, DateTime timestamp, Location location, string userId, string userName, string message)
           : base(id, type, timestamp, location, userId, userName)
        {
            this.Message = message;
        }

        public static NotificationActivity Create(
            string userId,
            string userName,
            string message)
        {
            var timestamp = DateTime.Now;
            var id = ActivityId.Create(timestamp, userId);

            var activity = new NotificationActivity(id.Value, ActivityType.Notification, timestamp,
                Location.Nowhere, userId, userName, message);

            return activity;
        }

        public static NotificationActivity Create(
            string id,
            DateTime timestamp,
            string userId,
            string userName,
            string message)
        {
            var activity = new NotificationActivity(id, ActivityType.Notification, timestamp,
                Location.Nowhere, userId, userName, message);

            return activity;
        }
    }
}
