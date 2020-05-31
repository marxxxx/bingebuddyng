using System;

namespace BingeBuddyNg.Services.Activity.Domain
{
    public class ProfileImageUpdateActivity : Activity
    {
        private ProfileImageUpdateActivity(string id, ActivityType type, DateTime timestamp, Location location, string userId, string userName) 
            : base(id, type, timestamp, location, userId, userName)
        {
        }

        public static ProfileImageUpdateActivity Create(string userId, string userName)
        {
            var timestamp = DateTime.Now;
            var id = ActivityId.Create(timestamp, userId);

            var activity = new ProfileImageUpdateActivity(
                id.Value,
                ActivityType.ProfileImageUpdate,
                timestamp,
                Location.Nowhere,
                userId,
                userName);

            return activity;
        }

        public static ProfileImageUpdateActivity Create(string id, string userId, string userName)
        {
            var activity = new ProfileImageUpdateActivity(
                id,
                ActivityType.ProfileImageUpdate, 
                DateTime.UtcNow, 
                Location.Nowhere,
                userId, 
                userName);

            return activity;
        }
    }
}
