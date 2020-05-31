using System.Collections.Generic;
using System.Linq;

namespace BingeBuddyNg.Services.Activity
{
    public static class Converter
    {
        public static ActivityDTO ToDto(this Activity a)
        {
            return new ActivityDTO()
            {
                Id = a.Id,
                ActivityType = a.ActivityType,
                Timestamp = a.Timestamp,
                Location = a.Location,
                LocationAddress = a.LocationAddress,
                UserId = a.UserId,
                UserName = a.UserName,
                Message = a.Message,
                DrinkType = a.DrinkType,
                DrinkId = a.DrinkId,
                DrinkName = a.DrinkName,
                DrinkAlcPrc = a.DrinkAlcPrc,
                DrinkVolume = a.DrinkVolume,
                DrinkCount = a.DrinkCount,
                AlcLevel = a.AlcLevel,
                ImageUrl = a.ImageUrl,
                CountryLongName = a.CountryLongName,
                CountryShortName = a.CountryShortName,
                Venue = a.Venue,
                RegistrationUser = a.RegistrationUser,
                OriginalUserName = a.OriginalUserName,
                GameInfo = a.GameInfo,
                Likes = a.Likes?.Select(l => new ReactionDTO() { Timestamp = l.Timestamp, UserId = l.UserId, UserName = l.UserName }).ToList(),
                Cheers = a.Cheers?.Select(c => new ReactionDTO() { Timestamp = c.Timestamp, UserId = c.UserId, UserName = c.UserName }).ToList(),
                Comments = a.Comments?.Select(c => new CommentReactionDTO() { Timestamp = c.Timestamp, UserId = c.UserId, UserName = c.UserName, Comment = c.Comment }).ToList()
            };
        }

        public static List<Activity> ToActivityListWithAdjustedIds(this IEnumerable<ActivityTableEntity> result)
        {
            List<Activity> resultActivities = new List<Activity>();
            foreach (var r in result)
            {
                r.Entity.Id = r.RowKey;
                resultActivities.Add(r.Entity);
            }
            return resultActivities;
        }
    }
}
