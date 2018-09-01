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
        public DrinkType DrinkType { get; set; }
        public string DrinkId { get; set; }
        public string DrinkName { get; set; }
        public double? DrinkAlcPrc { get; set; }
        public double? DrinkVolume { get; set; }
        public string ImageUrl { get; set; }
        public string CountryLongName { get; set; }
        public string CountryShortName { get; set; }

        public List<Reaction> Likes { get; set; } = new List<Reaction>();
        public List<Reaction> Cheers { get; set; } = new List<Reaction>();
        public List<CommentReaction> Comments { get; set; } = new List<CommentReaction>();
        
        public Activity()
        { }

        public Activity(ActivityType type, DateTime timestamp, Location location,
            string userId, string userName, string userProfileImageUrl)
            : this(null, type, timestamp, location, userId, userName, userProfileImageUrl)
        {
        }

        public Activity(string id, ActivityType type, DateTime timestamp, Location location, 
            string userId, string userName, string userProfileImageUrl)
        {
            this.Id = id;
            this.ActivityType = type;
            this.Timestamp = timestamp;
            this.Location = location;
            this.UserId = userId;
            this.UserName = userName;
            this.UserProfileImageUrl = userProfileImageUrl;
        }

        public static Activity CreateDrinkActivity(DateTime activityTimestamp,
           Location location, string userId, string userName, string userProfileImageUrl,
           DrinkType drinkType, string drinkId, string drinkName,
           double drinkAlcPrc, double drinkVolume)
        {
            var activity = new Activity( ActivityType.Drink, activityTimestamp,
                location, userId, userName, userProfileImageUrl)
            {
                DrinkType = drinkType,
                DrinkId = drinkId,
                DrinkName = drinkName,
                DrinkAlcPrc = drinkAlcPrc,
                DrinkVolume = drinkVolume
            };

            return activity;
        }

        public static Activity CreateMessageActivity(DateTime activityTimestamp,
           Location location, string userId, string userName, string userProfileImageUrl, string message)
        {
            var activity = new Activity(ActivityType.Message, activityTimestamp,
                location, userId, userName, userProfileImageUrl)
            {
                Message = message
            };

            return activity;
        }

        public static Activity CreateImageActivity(DateTime activityTimestamp,
                    Location location, string userId, string userName, string userProfileImageUrl, string imageUrl)
        {
            var activity = new Activity(ActivityType.Image, activityTimestamp,
                location, userId, userName, userProfileImageUrl)
            {
                ImageUrl = imageUrl
            };

            return activity;
        }

        public void AddComment(CommentReaction reaction)
        {
            this.Comments.Add(reaction);
        }

        public void AddLike(Reaction reaction)
        {
            this.Likes.Add(reaction);
        }

        public void AddCheers(Reaction reaction)
        {
            this.Cheers.Add(reaction);
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(ActivityType)}: {ActivityType}, {nameof(Timestamp)}: {Timestamp}, {nameof(UserName)}: {UserName}, {nameof(Message)}: {Message}";
        }
    }
}
