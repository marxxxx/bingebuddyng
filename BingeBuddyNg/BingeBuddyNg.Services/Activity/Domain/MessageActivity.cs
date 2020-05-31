using System;
using BingeBuddyNg.Services.Activity.Domain.Events;

namespace BingeBuddyNg.Services.Activity.Domain
{
    public class MessageActivity : Activity
    {
        public string Message { get; private set; }

        private MessageActivity(string id, ActivityType type, DateTime timestamp, Location location, string userId, string userName, string message, Venue.Venue venue)
            : base(id, type, timestamp, location, userId, userName)
        {
            this.Message = message;
            this.Venue = venue;
        }

        public static MessageActivity Create(
            Location location,
            string userId,
            string userName,
            string message,
            Venue.Venue venue)
        {
            var timestamp = DateTime.Now;
            var id = ActivityId.Create(timestamp, userId);

            var activity = new MessageActivity(
                id.Value,
                ActivityType.Message,
                timestamp,
                location,
                userId,
                userName,
                message,
                venue);

            return activity;
        }

        public static MessageActivity Create(
            string id,
            DateTime timestamp,
            Location location,
            string userId,
            string userName,
            string message,
            Venue.Venue venue)
        {
            var activity = new MessageActivity(
                id,
                ActivityType.Message,
                timestamp,
                location,
                userId,
                userName,
                message,
                venue);

            return activity;
        }
    }
}
