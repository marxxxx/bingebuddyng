using BingeBuddyNg.Services.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Entitys
{
    public class ActivityTableEntity : JsonTableEntity<Activity>
    {
        public ActivityType ActivityType { get; set; }
        public bool HasLocation { get; set; }

        public ActivityTableEntity()
        { }

        public ActivityTableEntity(string partitionKey, string rowKey, Activity activity) : base(partitionKey, rowKey, activity)
        {
            this.ActivityType = activity.ActivityType;
            this.HasLocation = activity.Location != null && activity.Location.IsValid();
        }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            base.ReadEntity(properties, operationContext);
            if (properties.TryGetValue(nameof(ActivityType), out EntityProperty activityTypeValue))
            {
                this.ActivityType = Util.SafeParseEnum<ActivityType>(activityTypeValue.StringValue, ActivityType.None);
            }

            if (properties.TryGetValue(nameof(HasLocation), out EntityProperty hasLocationValue))
            {
                this.HasLocation = activityTypeValue.BooleanValue.GetValueOrDefault();
            }

        }

        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            var properties = base.WriteEntity(operationContext);
            properties.Add(nameof(ActivityType), new EntityProperty(ActivityType.ToString()));
            properties.Add(nameof(HasLocation), new EntityProperty(HasLocation));
            return properties;
        }
    }
}
