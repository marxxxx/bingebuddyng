using System;
using System.Linq;
using BingeBuddyNg.Services.Activity.Domain;

namespace BingeBuddyNg.Services.Activity
{
    public static class Converter
    {
        public static Activity ToDomain(this ActivityDTO a) 
        {
            switch(a.ActivityType)
            {
                case ActivityType.Drink:
                    {
                        return DrinkActivity.Create(a.Id, a.Timestamp, a.Location, a.UserId, a.UserName, a.DrinkType, a.DrinkId, a.DrinkName, a.DrinkAlcPrc.GetValueOrDefault(), a.DrinkVolume.GetValueOrDefault(), a.Venue);
                    }
                case ActivityType.GameResult:
                    {
                        return GameResultActivity.Create(a.Id, a.Timestamp, a.GameInfo, new User.UserInfo(a.UserId, a.UserName));
                    }
                case ActivityType.Image:
                    {
                        return ImageActivity.Create(a.Id, a.Timestamp, a.Location, a.UserId, a.UserName, a.ImageUrl);
                    }
                case ActivityType.Message:
                    {
                        return MessageActivity.Create(a.Id, a.Timestamp, a.Location, a.UserId, a.UserName, a.Message, a.Venue);
                    }
                case ActivityType.Notification:
                    {
                        return NotificationActivity.Create(a.Id, a.Timestamp, a.UserId, a.UserName, a.Message);
                    }
                case ActivityType.ProfileImageUpdate:
                    {
                        return ProfileImageUpdateActivity.Create(a.Id, a.Timestamp, a.UserId, a.UserName);
                    }
                case ActivityType.Registration:
                    {
                        return RegistrationActivity.Create(a.Id, a.Timestamp, a.UserId, a.UserName, a.RegistrationUser);
                    }
                case ActivityType.Rename:
                    {
                        return RenameActivity.Create(a.Id, a.Timestamp, a.UserId, a.UserName, a.OriginalUserName);
                    }
                case ActivityType.VenueEntered:
                    {
                        return VenueActivity.Create(a.Id, a.Timestamp, a.UserId, a.UserName, a.Venue, Venue.VenueAction.Enter);
                    }
                case ActivityType.VenueLeft:
                    {
                        return VenueActivity.Create(a.Id, a.Timestamp, a.UserId, a.UserName, a.Venue, Venue.VenueAction.Leave);
                    }
                default:
                    {
                        throw new InvalidOperationException("Invalid activity type");
                    }
            }
        }

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
