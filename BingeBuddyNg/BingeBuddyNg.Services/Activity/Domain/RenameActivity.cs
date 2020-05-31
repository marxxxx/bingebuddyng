using System;
using Newtonsoft.Json;

namespace BingeBuddyNg.Services.Activity.Domain
{
    public class RenameActivity : Activity
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string OriginalUserName { get; private set; }

        private RenameActivity(string id, ActivityType type, DateTime timestamp, Location location, string userId, string userName, string originalUserName)
            : base(id, type, timestamp, location, userId, userName)
        {
            this.OriginalUserName = originalUserName ?? throw new ArgumentNullException(nameof(originalUserName));
        }

        public static Activity Create(string userId, string userName, string originalUserName)
        {
            var timestamp = DateTime.Now;
            var id = ActivityId.Create(timestamp, userId);
            var activity = new RenameActivity(id.Value, ActivityType.Rename, DateTime.UtcNow, Location.Nowhere,
                userId, userName, originalUserName);

            return activity;
        }

        public static Activity Create(string id, DateTime timestamp, string userId, string userName, string originalUserName)
        {
            var activity = new RenameActivity(id, ActivityType.Rename, timestamp, Location.Nowhere,
                userId, userName, originalUserName);

            return activity;
        }
    }
}
