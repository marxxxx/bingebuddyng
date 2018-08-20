using BingeBuddyNg.Services.Models;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Entitys
{
    public class ActivityTableEntity : TableEntity
    {
        public DateTime ActivityTimestamp { get; set; }
        public string ActivityType { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserProfileImageUrl { get; set; }
        public string Message { get; set; }
        public string DrinkType { get; set; }
        public string DrinkId { get; set; }
        public string DrinkName { get; set; }
        public double? DrinkAlcPrc { get; set; }
        public double? DrinkMl { get; set; }
        public string ImageUrl { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string LocationAddress { get; set; }

        public ActivityTableEntity()
        { }

        //public static ActivityTableEntity CreateDrinkActivity(string partitionKey, string rowKey, DateTime activityTimestamp,
        //    double? latitude, double? longitude, string userId, string userName, string userProfileImageUrl, 
        //    DrinkType drinkType, string drinkId, string drinkName,
        //    double drinkAlcPrc, double drinkMl)
        //{
        //    var activity = new ActivityTableEntity(partitionKey, rowKey, activityTimestamp, BingeBuddyNg.Services.Models.ActivityType.Drink,
        //        latitude, longitude, userId, userName, userProfileImageUrl)
        //    {
        //        DrinkType = drinkType.ToString(),
        //        DrinkId = drinkId,
        //        DrinkName = drinkName,
        //        DrinkAlcPrc = drinkAlcPrc,
        //        DrinkMl = drinkMl
        //    };

        //    return activity;
        //}

        //public static ActivityTableEntity CreateMessageActivity(string partitionKey, string rowKey, DateTime activityTimestamp,
        //    double? latitude, double? longitude, string userId, string userName, string userProfileImageUrl, string message)
        //{
        //    var activity = new ActivityTableEntity(partitionKey, rowKey, activityTimestamp, BingeBuddyNg.Services.Models.ActivityType.Drink,
        //        latitude, longitude, userId, userName, userProfileImageUrl)
        //    {
        //        Message = message
        //    };

        //    return activity;
        //}

        //public static ActivityTableEntity CreateImageActivity(string partitionKey, string rowKey, DateTime activityTimestamp,
        //            double? latitude, double? longitude, string userId, string userName, string userProfileImageUrl, string imageUrl)
        //{
        //    var activity = new ActivityTableEntity(partitionKey, rowKey, activityTimestamp, BingeBuddyNg.Services.Models.ActivityType.Drink,
        //        latitude, longitude, userId, userName, userProfileImageUrl)
        //    {
        //        ImageUrl = imageUrl
        //    };

        //    return activity;
        //}



        public ActivityTableEntity(string partitionKey, string rowKey, DateTime activityTimestamp, ActivityType type,
            double? latitude, double? longitude,
            string userId, string userName, string userProfileImageUrl)
        {
            this.PartitionKey = partitionKey;
            this.RowKey = rowKey;

            this.ActivityTimestamp = activityTimestamp;
            this.ActivityType = type.ToString();
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.UserId = userId;
            this.UserName = userName;
            this.UserProfileImageUrl = userProfileImageUrl;

        }
    }
}
