using System;
using BingeBuddyNg.Services.Venue;

namespace BingeBuddyNg.Services.Activity.Domain
{
    public class VenueActivity : Activity
    {
        private VenueActivity(string id, ActivityType type, DateTime timestamp, Location location, string userId, string userName, Venue.Venue venue) : base(id, type, timestamp, location, userId, userName)
        {
            this.Venue = venue ?? throw new ArgumentNullException(nameof(venue));
        }

        public static VenueActivity Create(
            string userId, string userName, Venue.Venue venue, VenueAction action)
        {            
            if (action == VenueAction.Unknown)
            {
                throw new ArgumentException("Invalid venue action!");
            }

            var timestamp = DateTime.Now;
            var id = ActivityId.Create(timestamp, userId);

            var activity = new VenueActivity(id.Value, action == VenueAction.Enter ? ActivityType.VenueEntered : ActivityType.VenueLeft,
                timestamp, venue.Location, userId, userName, venue);

            return activity;
        }

        public static VenueActivity Create(string id, DateTime activityTimestamp,
           string userId, string userName, Venue.Venue venue, VenueAction action)
        {
            if (action == VenueAction.Unknown)
            {
                throw new ArgumentException("Invalid venue action!");
            }

            var activity = new VenueActivity(id, action == VenueAction.Enter ? ActivityType.VenueEntered : ActivityType.VenueLeft,
                activityTimestamp, venue.Location, userId, userName, venue);

            return activity;
        }
    }
}
