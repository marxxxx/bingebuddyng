using System;
using System.Linq;
using BingeBuddyNg.Services.Activity.Persistence;
using BingeBuddyNg.Services.Game;
using BingeBuddyNg.Services.Venue;

namespace BingeBuddyNg.Services.Activity
{
    public static class Converter
    {
        public static ActivityDTO ToDto(this ActivityEntity a)
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
                DrinkCount = a.DrinkCount,
                AlcLevel = a.AlcLevel,
                CountryLongName = a.CountryLongName,
                CountryShortName = a.CountryShortName,
                Venue = new Venue.Venue(id: a.Venue.Id, location: a.Venue.Location, name: a.Venue.Name, distance: a.Venue.Distance),
                Likes = a.Likes.ConvertAll(aLike => new ReactionDTO()
                {
                    Timestamp = aLike.Timestamp,
                    UserId = aLike.UserId,
                    UserName = aLike.UserName
                }),
                Cheers = a.Cheers.ConvertAll(aCheer => new ReactionDTO()
                {
                    Timestamp = aCheer.Timestamp,
                    UserId = aCheer.UserId,
                    UserName = aCheer.UserName
                }),
                Comments = a.Comments.ConvertAll(aComment => new CommentReactionDTO()
                {
                    Comment = aComment.Comment,
                    Timestamp = aComment.Timestamp,
                    UserId = aComment.UserId,
                    UserName = aComment.UserName
                })
            };
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

            switch (a.ActivityType)
            {
                case ActivityType.Drink:
                    {
                        dto.DrinkType = a.Drink.DrinkType;
                        dto.DrinkId = a.Drink.DrinkId;
                        dto.DrinkName = a.Drink.DrinkName;
                        dto.DrinkAlcPrc = a.Drink.DrinkAlcPrc;
                        dto.DrinkVolume = a.Drink.DrinkVolume;
                        break;
                    }
                case ActivityType.Image:
                    {
                        dto.ImageUrl = a.Image.ImageUrl;
                        break;
                    }
                case ActivityType.GameResult:
                    {
                        dto.GameInfo = a.Game.GameInfo.ToDto();
                        break;
                    }
                case ActivityType.Rename:
                    {
                        dto.OriginalUserName = a.Rename.OriginalUserName;
                        break;
                    }
                case ActivityType.Registration:
                    {
                        dto.RegistrationUser = a.Registration.RegistrationUser;
                        break;
                    }
                case ActivityType.Message:
                    {
                        dto.Message = a.Message.Message;
                        break;
                    }
                case ActivityType.Notification:
                    {
                        dto.Message = a.Notification.Message;
                        break;
                    }
            }

            return dto;
        }

        public static ActivityEntity ToEntity(this Activity a)
        {
            var entity = new ActivityEntity()
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
                Venue = a.Venue?.ToEntity(),
                Likes = a.Likes?.Select(l => new Reaction() { Timestamp = l.Timestamp, UserId = l.UserId, UserName = l.UserName }).ToList(),
                Cheers = a.Cheers?.Select(c => new Reaction() { Timestamp = c.Timestamp, UserId = c.UserId, UserName = c.UserName }).ToList(),
                Comments = a.Comments?.Select(c => new CommentReaction() { Timestamp = c.Timestamp, UserId = c.UserId, UserName = c.UserName, Comment = c.Comment }).ToList()
            };

            switch (a.ActivityType)
            {
                case ActivityType.Drink:
                    {
                        entity.DrinkType = a.Drink.DrinkType;
                        entity.DrinkId = a.Drink.DrinkId;
                        entity.DrinkName = a.Drink.DrinkName;
                        entity.DrinkAlcPrc = a.Drink.DrinkAlcPrc;
                        entity.DrinkVolume = a.Drink.DrinkVolume;
                        break;
                    }
                case ActivityType.Image:
                    {
                        entity.ImageUrl = a.Image.ImageUrl;
                        break;
                    }
                case ActivityType.GameResult:
                    {
                        entity.GameInfo = a.Game.GameInfo;
                        break;
                    }
                case ActivityType.Rename:
                    {
                        entity.OriginalUserName = a.Rename.OriginalUserName;
                        break;
                    }
                case ActivityType.Registration:
                    {
                        entity.RegistrationUser = a.Registration.RegistrationUser;
                        break;
                    }
                case ActivityType.Message:
                    {
                        entity.Message = a.Message.Message;
                        break;
                    }
                case ActivityType.Notification:
                    {
                        entity.Message = a.Notification.Message;
                        break;
                    }
            }

            return entity;
        }

        public static Activity ToDomain(this ActivityEntity entity)
        {
            switch (entity.ActivityType)
            {
                case ActivityType.Drink:
                    {
                        return Activity.CreateDrinkActivity(
                            entity.Id,
                            entity.Timestamp,
                            entity.Location,
                            entity.UserId,
                            entity.UserName,
                            entity.DrinkType,
                            entity.DrinkId,
                            entity.DrinkName,
                            entity.DrinkAlcPrc.GetValueOrDefault(),
                            entity.DrinkVolume.GetValueOrDefault(),
                            entity.Venue?.ToDomain());
                    }
                case ActivityType.GameResult:
                    {
                        return Activity.CreateGameActivity(
                            entity.Id,
                            entity.Timestamp,
                            entity.GameInfo,
                            new User.UserInfo(entity.UserId, entity.UserName));
                    }
                case ActivityType.Image:
                    {
                        return Activity.CreateImageActivity(
                            entity.Id,
                            entity.Timestamp,
                            entity.Location,
                            entity.UserId,
                            entity.UserName,
                            entity.ImageUrl);
                    }
                case ActivityType.Message:
                    {
                        return Activity.CreateMessageActivity(
                            entity.Id,
                            entity.Timestamp,
                            entity.Location,
                            entity.UserId,
                            entity.UserName,
                            entity.Message,
                            entity.Venue?.ToDomain());
                    }
                case ActivityType.Notification:
                    {
                        return Activity.CreateNotificationActivity(
                            entity.Id,
                            entity.Timestamp,
                            entity.UserId,
                            entity.UserName,
                            entity.Message);
                    }
                case ActivityType.ProfileImageUpdate:
                    {
                        return Activity.CreateProfileImageUpdateActivity(
                            entity.Id,
                            entity.Timestamp,
                            entity.UserId,
                            entity.UserName);
                    }
                case ActivityType.Registration:
                    {
                        return Activity.CreateRegistrationActivity(
                            entity.Id,
                            entity.Timestamp,
                            entity.UserId,
                            entity.UserName,
                            entity.RegistrationUser);
                    }
                case ActivityType.Rename:
                    {
                        return Activity.CreateRenameActivity(
                            entity.Id,
                            entity.Timestamp,
                            entity.UserId,
                            entity.UserName,
                            entity.OriginalUserName);
                    }
                case ActivityType.VenueEntered:
                    {
                        return Activity.CreateVenueActivity(
                            entity.Id,
                            entity.Timestamp,
                            entity.UserId,
                            entity.UserName,
                            entity.Venue?.ToDomain(),
                            VenueAction.Enter);
                    }
                case ActivityType.VenueLeft:
                    {
                        return Activity.CreateVenueActivity(
                            entity.Id,
                            entity.Timestamp,
                            entity.UserId,
                            entity.UserName,
                            entity.Venue?.ToDomain(),
                            VenueAction.Leave);
                    }
                default:
                    {
                        throw new InvalidOperationException("Invalid activity type " + entity.ActivityType);
                    }
            }
        }
    }
}
