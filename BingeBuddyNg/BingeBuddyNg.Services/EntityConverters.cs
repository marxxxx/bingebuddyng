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

                return new User(entity.Id, entity.DisplayName, entity.ProfileImageUrl, Util.SafeParseEnum<Gender>(entity.Gender, Gender.Unknown), entity.Weight);
            }

            public static UserTableEntity ModelToEntity(User model)
            {
                if (model == null)
                    throw new ArgumentNullException(nameof(model));

                return new UserTableEntity(model.Id, model.Name, model.ProfileImageUrl, model.Gender, model.Weight);

            }
        }

        public static class Activitys
        {
            public static Activity EntityToModel(ActivityTableEntity entity)
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                var model = new Activity(entity.RowKey,
                    Util.SafeParseEnum<ActivityType>(entity.ActivityType, ActivityType.None),
                    entity.ActivityTimestamp,
                    entity.Latitude != null && entity.Longitude != null ? new Location(entity.Latitude.Value, entity.Longitude.Value) : null,
                    entity.UserId, entity.UserName, entity.UserProfileImageUrl)
                {
                    LocationAddress = entity.LocationAddress,
                    Message = entity.Message,
                    DrinkType = Util.SafeParseEnum<DrinkType>(entity.DrinkType, DrinkType.Unknown),
                    DrinkId = entity.DrinkId,
                    DrinkName = entity.DrinkName,
                    DrinkAlcPrc = entity.DrinkAlcPrc,
                    DrinkVolume = entity.DrinkMl,
                    ImageUrl = entity.ImageUrl
                };

                return model;
            }

            public static ActivityTableEntity ModelToEntity(Activity activity, string partitionKey, string rowKey)
            {
                if (activity == null)
                    throw new ArgumentNullException(nameof(activity));

                var activityEntity = new ActivityTableEntity(partitionKey, rowKey, activity.Timestamp, activity.ActivityType,
                    activity.Location?.Latitude, activity.Location?.Longitude,
                    activity.UserId, activity.UserName, activity.UserProfileImageUrl)
                {
                    DrinkId = activity.DrinkId,
                    DrinkType = activity.DrinkType.ToString(),
                    DrinkName = activity.DrinkName,
                    DrinkMl = activity.DrinkVolume,
                    DrinkAlcPrc = activity.DrinkAlcPrc,
                    Message = activity.Message,
                    ImageUrl = activity.ImageUrl
                };

                return activityEntity;
            }
        }
    }
}
