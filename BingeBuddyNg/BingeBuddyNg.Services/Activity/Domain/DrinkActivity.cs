using System;
using BingeBuddyNg.Services.Drink;

namespace BingeBuddyNg.Services.Activity.Domain
{
    public class DrinkActivity : Activity
    {
        private DrinkActivity(string id, ActivityType type, DateTime timestamp, Location location, string userId, string userName)
            : base(id, type, timestamp, location, userId, userName)
        {
        }

        public DrinkType DrinkType { get; private set; }
        public string DrinkId { get; private set; }
        public string DrinkName { get; private set; }
        public double? DrinkAlcPrc { get; private set; }
        public double? DrinkVolume { get; private set; }

        public static DrinkActivity Create(
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
            var activity = new DrinkActivity(id, ActivityType.Drink, activityTimestamp, location, userId, userName)
            {
                DrinkType = drinkType,
                DrinkId = drinkId,
                DrinkName = drinkName,
                DrinkAlcPrc = drinkAlcPrc,
                DrinkVolume = drinkVolume,
                Venue = venue
            };

            return activity;
        }
    }
}
