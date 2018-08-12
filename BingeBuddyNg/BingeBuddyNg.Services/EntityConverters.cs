using BingeBuddyNg.Services.Entitys;
using BingeBuddyNg.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services
{
    public static class EntityConverters
    {
        public static class Users
        {
            public static User EntityToModel(UserTableEntity entity)
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                return new User(entity.Id, entity.DisplayName, entity.ProfileImageUrl, entity.Weight);
            }

            public static UserTableEntity ModelToEntity(User model)
            {
                if (model == null)
                    throw new ArgumentNullException(nameof(model));

                return new UserTableEntity(model.Id, model.Name, model.ProfileImageUrl, model.Weight);

            }
        }

        public static class Activitys
        {
            public static Activity EntityToModel(ActivityTableEntity entity)
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                var model = new Activity(entity.RowKey,
                    (ActivityType)Enum.Parse(typeof(ActivityType), entity.ActivityType),
                    entity.ActivityTimestamp,
                    entity.Latitude != null && entity.Longitude != null ? new Location(entity.Latitude.Value, entity.Longitude.Value) : null,
                    entity.UserId, entity.UserName, entity.UserProfileImageUrl,
                    entity.Message, entity.DrinkName, entity.ImageUrl)
                {
                    LocationAddress = entity.LocationAddress
                };

                return model;
            }

            public static ActivityTableEntity ModelToEntity(Activity activity, string partitionKey, string rowKey)
            {
                if (activity == null)
                    throw new ArgumentNullException(nameof(activity));

                return new ActivityTableEntity(partitionKey, rowKey, activity.Timestamp, activity.ActivityType, 
                    activity.Location?.Latitude, activity.Location?.Longitude,
                    activity.UserId, activity.UserName, activity.UserProfileImageUrl,
                    activity.Message, activity.DrinkName, activity.ImageUrl);
            }
        }
    }
}
