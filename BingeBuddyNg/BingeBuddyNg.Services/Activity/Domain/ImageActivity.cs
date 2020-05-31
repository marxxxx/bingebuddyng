using System;

namespace BingeBuddyNg.Services.Activity.Domain
{
    public class ImageActivity : Activity
    {
        public string ImageUrl { get; private set; }

        private ImageActivity(string id, ActivityType type, DateTime timestamp, Location location, string userId, string userName, string imageUrl) : base(id, type, timestamp, location, userId, userName)
        {
            this.ImageUrl = imageUrl ?? throw new ArgumentNullException(nameof(imageUrl));
        }

        public static ImageActivity Create(string id, DateTime activityTimestamp,
                    Location location, string userId, string userName, string imageUrl)
        {
            var activity = new ImageActivity(id, ActivityType.Image, activityTimestamp,
                location, userId, userName, imageUrl);

            return activity;
        }
    }
}
