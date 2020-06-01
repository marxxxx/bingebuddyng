using System;
using System.Collections.Generic;
using BingeBuddyNg.Services.Activity.Domain;
using BingeBuddyNg.Services.Infrastructure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace BingeBuddyNg.Services.Activity
{
    public class ActivityTableEntity : JsonTableEntity<Activity>
    {
        public ActivityType ActivityType { get; set; }
        public bool HasLocation { get; set; }
        public string UserId { get; set; }
        public int DrinkCount { get; set; }
        public double? AlcLevel { get; set; }
        public string VenueId { get; set; }
        public string VenueName { get; set; }

        public ActivityTableEntity()
        { }

        public ActivityTableEntity(string partitionKey, string rowKey, Activity activity) : base(partitionKey, rowKey, activity)
        {
            this.ActivityType = activity.ActivityType;
            this.HasLocation = activity.Location != null && activity.Location.IsValid();
            this.UserId = activity.UserId;
            this.DrinkCount = activity.DrinkCount;
            this.AlcLevel = activity.AlcLevel;

            if (activity.Venue != null)
            {
                this.VenueId = activity.Venue.Id;
                this.VenueName = activity.Venue.Name;
            }
        }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            base.ReadEntity(properties, operationContext);
            if (properties.TryGetValue(nameof(ActivityType), out EntityProperty activityTypeValue))
            {
                this.ActivityType = Util.SafeParseEnum<ActivityType>(activityTypeValue.StringValue, ActivityType.None);
            }
        }

        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            var properties = base.WriteEntity(operationContext);
            properties.Add(nameof(ActivityType), new EntityProperty(ActivityType.ToString()));
            return properties;
        }

        public override Activity DeserializeObject(IDictionary<string, EntityProperty> properties, EntityProperty jsonProperty)
        {
            var type = properties[nameof(ActivityType)];

            var activityType = (ActivityType)Enum.Parse(typeof(ActivityType), type.StringValue);

            switch(activityType)
            {
                case ActivityType.VenueLeft:
                    {
                        return JsonConvert.DeserializeObject<VenueActivity>(jsonProperty.StringValue);
                    }
                case ActivityType.VenueEntered:
                    {
                        return JsonConvert.DeserializeObject<VenueActivity>(jsonProperty.StringValue);
                    }
                case ActivityType.Drink:
                    {
                        return JsonConvert.DeserializeObject<DrinkActivity>(jsonProperty.StringValue);
                    }
                default:
                    throw new InvalidOperationException("Unknown activity type");
            }
        }
    }
}
