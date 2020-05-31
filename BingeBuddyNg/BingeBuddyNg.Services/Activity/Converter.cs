using System.Linq;
using BingeBuddyNg.Services.Activity.Domain;

namespace BingeBuddyNg.Services.Activity
{
    public static class Converter
    {
        public static ActivityDTO ToDto(this Activity a)
        {
            var dto = new ActivityDTO()
            {
                Id = a.Id,
                ActivityType = a.ActivityType,
                Timestamp = a.Timestamp,
                Location = a.Location,
                LocationAddress = a.LocationAddress,
                UserId = a.UserId,
                UserName = a.UserName,              
                DrinkCount = a.DrinkCount,
                AlcLevel = a.AlcLevel,                
                CountryLongName = a.CountryLongName,
                CountryShortName = a.CountryShortName,
                Venue = a.Venue,
                Likes = a.Likes?.Select(l => new ReactionDTO() { Timestamp = l.Timestamp, UserId = l.UserId, UserName = l.UserName }).ToList(),
                Cheers = a.Cheers?.Select(c => new ReactionDTO() { Timestamp = c.Timestamp, UserId = c.UserId, UserName = c.UserName }).ToList(),
                Comments = a.Comments?.Select(c => new CommentReactionDTO() { Timestamp = c.Timestamp, UserId = c.UserId, UserName = c.UserName, Comment = c.Comment }).ToList()
            };

            if(a is DrinkActivity da)
            {
                dto.DrinkType = da.DrinkType;
                dto.DrinkId = da.DrinkId;
                dto.DrinkName = da.DrinkName;
                dto.DrinkAlcPrc = da.DrinkAlcPrc;
                dto.DrinkVolume = da.DrinkVolume;
            }

            if(a is ImageActivity ia)
            {
                dto.ImageUrl = ia.ImageUrl;
            }

            if(a is GameResultActivity ga)
            {
                dto.GameInfo = ga.GameInfo;
            }

            if(a is RenameActivity ra)
            {
                dto.OriginalUserName = ra.OriginalUserName;
            }

            if(a is RegistrationActivity rea)
            {
                dto.RegistrationUser = rea.RegistrationUser;
            }

            if (a is MessageActivity ma)
            {
                dto.Message = ma.Message;
            }

            if (a is NotificationActivity na)
            {
                dto.Message = na.Message;
            }

            return dto;
        }
    }
}
