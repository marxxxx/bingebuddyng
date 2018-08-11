using BingeBuddyNg.Services.Models;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Entitys
{
    public class ActivityTableEntity : TableEntity
    {
        public const string PartitionKeyValue = "ActivityEntity";

        public DateTime ActivityTimestamp { get; set; }
        public string ActivityType { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserProfileImageUrl { get; set; }
        public string Message { get; set; }
        public string DrinkName { get; set; }
        public string ImageUrl { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public ActivityTableEntity()
        { }

        public ActivityTableEntity(DateTime activityTimestamp, ActivityType type, 
            double? latitude, double? longitude,
            string userId, string userName, string userProfileImageUrl, string message, string drinkName, string imageUrl)
        {
            this.PartitionKey = PartitionKeyValue;
            this.RowKey = activityTimestamp.ToString("yyyyMMddHHmmss");

            this.ActivityTimestamp = activityTimestamp;
            this.ActivityType = type.ToString();
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.UserId = userId;
            this.UserName = userName;
            this.UserProfileImageUrl = userProfileImageUrl;
            this.Message = message;
            this.DrinkName = drinkName;
            this.ImageUrl = imageUrl;
        }
    }
}
