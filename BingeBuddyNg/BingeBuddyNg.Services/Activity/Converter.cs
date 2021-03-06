﻿using System;
using System.Linq;
using BingeBuddyNg.Core.Activity.Domain;
using BingeBuddyNg.Core.Activity.DTO;
using BingeBuddyNg.Core.Activity.Persistence;
using BingeBuddyNg.Core.Game;
using BingeBuddyNg.Core.User;
using BingeBuddyNg.Core.User.Persistence;
using BingeBuddyNg.Core.Venue;

namespace BingeBuddyNg.Core.Activity
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
                DrinkType = a.DrinkType,
                DrinkAlcPrc = a.DrinkAlcPrc,
                DrinkId = a.DrinkId,
                DrinkName = a.DrinkName,
                DrinkVolume = a.DrinkVolume,
                GameInfo = a.GameInfo?.ToDto(),
                ImageUrl = a.ImageUrl,
                Message = a.Message,
                OriginalUserName = a.OriginalUserName,
                RegistrationUser = a.RegistrationUser?.ToDto(),
                DrinkCount = a.DrinkCount,
                AlcLevel = a.AlcLevel,
                CountryLongName = a.CountryLongName,
                CountryShortName = a.CountryShortName,
                Venue = a.Venue?.ToDto(),
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

        public static ActivityDTO ToDto(this Domain.Activity a)
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
                Venue = a.Venue?.ToDto(),
                Likes = a.Likes?.Select(l => new ReactionDTO() { Timestamp = l.Timestamp, UserId = l.UserId, UserName = l.UserName }).ToList(),
                Cheers = a.Cheers?.Select(c => new ReactionDTO() { Timestamp = c.Timestamp, UserId = c.UserId, UserName = c.UserName }).ToList(),
                Comments = a.Comments?.Select(c => new CommentReactionDTO() { Timestamp = c.Timestamp, UserId = c.UserId, UserName = c.UserName, Comment = c.Comment }).ToList()
            };

            switch (a.ActivityType)
            {
                case Domain.ActivityType.Drink:
                    {
                        dto.DrinkType = a.Drink.DrinkType;
                        dto.DrinkId = a.Drink.DrinkId;
                        dto.DrinkName = a.Drink.DrinkName;
                        dto.DrinkAlcPrc = a.Drink.DrinkAlcPrc;
                        dto.DrinkVolume = a.Drink.DrinkVolume;
                        break;
                    }
                case Domain.ActivityType.Image:
                    {
                        dto.ImageUrl = a.Image.ImageUrl;
                        break;
                    }
                case Domain.ActivityType.GameResult:
                    {
                        dto.GameInfo = a.Game.GameInfo.ToDto();
                        break;
                    }
                case Domain.ActivityType.Rename:
                    {
                        dto.OriginalUserName = a.Rename.OriginalUserName;
                        break;
                    }
                case Domain.ActivityType.Registration:
                    {
                        dto.RegistrationUser = a.Registration.RegistrationUser?.ToDto();
                        break;
                    }
                case Domain.ActivityType.Message:
                    {
                        dto.Message = a.Message.Message;
                        break;
                    }
                case Domain.ActivityType.Notification:
                    {
                        dto.Message = a.Notification.Message;
                        break;
                    }
            }

            return dto;
        }

        public static ActivityEntity ToEntity(this Domain.Activity a)
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

        public static Domain.Activity ToDomain(this ActivityEntity entity)
        {
            Domain.Activity activity;

            switch (entity.ActivityType)
            {
                case ActivityType.Drink:
                    {
                        activity = Domain.Activity.CreateDrinkActivity(
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
                        break;
                    }
                case ActivityType.GameResult:
                    {
                        activity = Domain.Activity.CreateGameActivity(
                            entity.Id,
                            entity.Timestamp,
                            entity.GameInfo,
                            new UserInfo(entity.UserId, entity.UserName));
                        break;
                    }
                case ActivityType.Image:
                    {
                        activity = Domain.Activity.CreateImageActivity(
                            entity.Id,
                            entity.Timestamp,
                            entity.Location,
                            entity.UserId,
                            entity.UserName,
                            entity.ImageUrl);
                        break;
                    }
                case ActivityType.Message:
                    {
                        activity = Domain.Activity.CreateMessageActivity(
                            entity.Id,
                            entity.Timestamp,
                            entity.Location,
                            entity.UserId,
                            entity.UserName,
                            entity.Message,
                            entity.Venue?.ToDomain());
                        break;
                    }
                case ActivityType.Notification:
                    {
                        activity = Domain.Activity.CreateNotificationActivity(
                            entity.Id,
                            entity.Timestamp,
                            entity.UserId,
                            entity.UserName,
                            entity.Message);
                        break;
                    }
                case ActivityType.ProfileImageUpdate:
                    {
                        activity = Domain.Activity.CreateProfileImageUpdateActivity(
                            entity.Id,
                            entity.Timestamp,
                            entity.UserId,
                            entity.UserName);
                        break;
                    }
                case ActivityType.Registration:
                    {
                        activity = Domain.Activity.CreateRegistrationActivity(
                            entity.Id,
                            entity.Timestamp,
                            entity.UserId,
                            entity.UserName,
                            entity.RegistrationUser);
                        break;
                    }
                case ActivityType.Rename:
                    {
                        activity = Domain.Activity.CreateRenameActivity(
                            entity.Id,
                            entity.Timestamp,
                            entity.UserId,
                            entity.UserName,
                            entity.OriginalUserName);
                        break;
                    }
                case ActivityType.VenueEntered:
                    {
                        activity = Domain.Activity.CreateVenueActivity(
                            entity.Id,
                            entity.Timestamp,
                            entity.UserId,
                            entity.UserName,
                            entity.Venue?.ToDomain(),
                            VenueAction.Enter);
                        break;
                    }
                case ActivityType.VenueLeft:
                    {
                        activity = Domain.Activity.CreateVenueActivity(
                            entity.Id,
                            entity.Timestamp,
                            entity.UserId,
                            entity.UserName,
                            entity.Venue?.ToDomain(),
                            VenueAction.Leave);
                        break;
                    }
                default:
                    {
                        throw new InvalidOperationException("Invalid activity type " + entity.ActivityType);
                    }
            }
            activity.UpdateStats(entity.DrinkCount, entity.AlcLevel.GetValueOrDefault());
            activity.UpdateLocation(entity.LocationAddress);

            foreach (var l in entity.Likes)
            {
                activity.AddLike(l);
            }

            foreach (var c in entity.Comments)
            {
                activity.AddComment(c);
            }

            foreach (var c in entity.Cheers)
            {
                activity.AddCheers(c);
            }

            return activity;
        }
    }
}