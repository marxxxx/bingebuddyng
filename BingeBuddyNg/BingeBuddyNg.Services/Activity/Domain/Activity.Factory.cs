using System;
using BingeBuddyNg.Core.Activity.Persistence;
using BingeBuddyNg.Services.Activity.Domain;
using BingeBuddyNg.Services.Drink;
using BingeBuddyNg.Services.Game.Persistence;
using BingeBuddyNg.Services.User;
using BingeBuddyNg.Services.Venue;

namespace BingeBuddyNg.Services.Activity
{
    public partial class Activity
    {
        public static Activity CreateDrinkActivity(
           Location location,
           string userId,
           string userName,
           DrinkType drinkType,
           string drinkId,
           string drinkName,
           double drinkAlcPrc,
           double drinkVolume,
           Venue.Venue venue)
        {
            var timestamp = DateTime.UtcNow;
            return CreateDrinkActivity(ActivityKeyFactory.CreateRowKey(timestamp, userId), timestamp, location, userId, userName, drinkType, drinkId, drinkName, drinkAlcPrc, drinkVolume, venue);
        }

        public static Activity CreateDrinkActivity(
            string id,
            DateTime activityTimestamp,
            Location location,
            string userId,
            string userName,
            DrinkType drinkType,
            string drinkId,
            string drinkName,
            double drinkAlcPrc,
            double drinkVolume,
            Venue.Venue venue)
        {
            var activity = new Activity(id, ActivityType.Drink, activityTimestamp, location, userId, userName)
            {
                Drink = new DrinkActivityInfo(drinkType, drinkId, drinkName, drinkAlcPrc, drinkVolume),
                Venue = venue
            };

            return activity;
        }

        public static Activity CreateGameActivity(GameEntity game, UserInfo winner)
        {
            var timestamp = DateTime.UtcNow;
            return CreateGameActivity(ActivityKeyFactory.CreateRowKey(timestamp, winner.UserId), timestamp, game, winner);
        }

        public static Activity CreateGameActivity(string id, DateTime timestamp, GameEntity game, UserInfo winner)
        {
            var activity = new Activity(id, ActivityType.GameResult, timestamp, Location.Nowhere, winner.UserId, winner.UserName)
            {
                Game = new GameResultActivityInfo(game)
            };

            return activity;
        }

        public static Activity CreateImageActivity(Location location, string userId, string userName, string imageUrl)
        {
            var timestamp = DateTime.UtcNow;
            return CreateImageActivity(ActivityKeyFactory.CreateRowKey(timestamp, userId), timestamp, location, userId, userName, imageUrl);
        }

        public static Activity CreateImageActivity(string id, DateTime activityTimestamp,
                    Location location, string userId, string userName, string imageUrl)
        {
            var activity = new Activity(id, ActivityType.Image, activityTimestamp,
                location, userId, userName)
            {
                Image = new ImageActivityInfo(imageUrl)
            };

            return activity;
        }

        public static Activity CreateMessageActivity(
           Location location,
           string userId,
           string userName,
           string message,
           Venue.Venue venue)
        {
            var timestamp = DateTime.UtcNow;
            return CreateMessageActivity(ActivityKeyFactory.CreateRowKey(timestamp, userId), timestamp, location, userId, userName, message, venue);
        }

        public static Activity CreateMessageActivity(
            string id,
            DateTime timestamp,
            Location location,
            string userId,
            string userName,
            string message,
            Venue.Venue venue)
        {
            var activity = new Activity(id, ActivityType.Message, timestamp, location, userId, userName)
            {
                Message = new MessageActivityInfo(message),
                Venue = venue
            };

            return activity;
        }

        public static Activity CreateNotificationActivity(
            string userId,
            string userName,
            string message)
        {
            var timestamp = DateTime.UtcNow;
            return CreateNotificationActivity(ActivityKeyFactory.CreateRowKey(timestamp, userId), timestamp, userId, userName, message);
        }

        public static Activity CreateNotificationActivity(
            string id,
            DateTime timestamp,
            string userId,
            string userName,
            string message)
        {
            var activity = new Activity(id, ActivityType.Notification, timestamp, Location.Nowhere, userId, userName)
            {
                Notification = new NotificationActivityInfo(message)
            };

            return activity;
        }

        public static Activity CreateProfileImageUpdateActivity(string userId, string userName)
        {
            var timestamp = DateTime.UtcNow;
            return CreateProfileImageUpdateActivity(ActivityKeyFactory.CreateRowKey(timestamp, userId), timestamp, userId, userName);
        }
        public static Activity CreateProfileImageUpdateActivity(string id, DateTime timestamp, string userId, string userName)
        {
            var activity = new Activity(
                id,
                ActivityType.ProfileImageUpdate,
                timestamp,
                Location.Nowhere,
                userId,
                userName);

            return activity;
        }

        public static Activity CreateRegistrationActivity(string userId, string userName, UserInfo registrationUser)
        {
            var timestamp = DateTime.UtcNow;
            return CreateRegistrationActivity(ActivityKeyFactory.CreateRowKey(timestamp, userId), timestamp, userId, userName, registrationUser);
        }

        public static Activity CreateRegistrationActivity(string id, DateTime timestamp, string userId, string userName, UserInfo registrationUser)
        {
            var activity = new Activity(id, ActivityType.Registration, timestamp, Location.Nowhere, userId, userName)
            {
                Registration = new RegistrationActivityInfo(registrationUser)
            };

            return activity;
        }

        public static Activity CreateRenameActivity(string userId, string userName, string originalUserName)
        {
            var timestamp = DateTime.UtcNow;
            return CreateRenameActivity(ActivityKeyFactory.CreateRowKey(timestamp, userId), timestamp, userId, userName, originalUserName);
        }

        public static Activity CreateRenameActivity(string id, DateTime timestamp, string userId, string userName, string originalUserName)
        {
            var activity = new Activity(id, ActivityType.Rename, timestamp, Location.Nowhere, userId, userName)
            {
                Rename = new RenameActivityInfo(originalUserName)
            };

            return activity;
        }

        public static Activity CreateVenueActivity(string userId, string userName, Venue.Venue venue, VenueAction action)
        {
            var timestamp = DateTime.UtcNow;
            return CreateVenueActivity(ActivityKeyFactory.CreateRowKey(timestamp, userId), timestamp, userId, userName, venue, action);
        }

        public static Activity CreateVenueActivity(string id, DateTime activityTimestamp, string userId, string userName, Venue.Venue venue, VenueAction action)
        {
            if (action == VenueAction.Unknown)
            {
                throw new ArgumentException("Invalid venue action!");
            }

            var activity = new Activity(id, action == VenueAction.Enter ? ActivityType.VenueEntered : ActivityType.VenueLeft,
                activityTimestamp, venue.Location, userId, userName)
            {
                Venue = venue
            };

            return activity;
        }
    }
}
