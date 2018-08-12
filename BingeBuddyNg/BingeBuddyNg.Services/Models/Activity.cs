using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Models
{
    public class Activity
    {
        public string Id { get; set; }
        public ActivityType ActivityType { get; set; }
        public DateTime Timestamp { get; set; }
        public Location Location { get; set; }
        public string LocationAddress { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserProfileImageUrl { get; set; }
        public string Message { get; set; }
        public string DrinkName { get; set; }
        public string ImageUrl { get; set; }
        public string CountryLongName { get; set; }
        public string CountryShortName { get; set; }

        public Activity()
        { }

        public Activity(ActivityType type, DateTime timestamp, Location location,
            string userId, string userName, string userProfileImageUrl, string message, string drinkName = null, string imageUrl = null)
            : this(null, type, timestamp, location, userId, userName, userProfileImageUrl, message, drinkName, imageUrl)
        {
        }

        public Activity(string id, ActivityType type, DateTime timestamp, Location location, 
            string userId, string userName, string userProfileImageUrl, string message, string drinkName = null, string imageUrl = null)
        {
            this.Id = id;
            this.ActivityType = type;
            this.Timestamp = timestamp;
            this.Location = location;
            this.UserId = userId;
            this.UserName = userName;
            this.UserProfileImageUrl = userProfileImageUrl;
            this.Message = message;
            this.DrinkName = drinkName;
            this.ImageUrl = imageUrl;
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(ActivityType)}: {ActivityType}, {nameof(Timestamp)}: {Timestamp}, {nameof(UserName)}: {UserName}, {nameof(Message)}: {Message}";
        }
    }
}
