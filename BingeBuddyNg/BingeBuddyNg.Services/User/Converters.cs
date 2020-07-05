using System.Linq;
using BingeBuddyNg.Core.User.DTO;
using BingeBuddyNg.Core.User.Persistence;
using BingeBuddyNg.Core.Venue;
using BingeBuddyNg.Core.Venue.Persistence;

namespace BingeBuddyNg.Core.User
{
    public static class Converters
    {
        public static UserInfo ToUserInfo(this Domain.User user)
        {
            return new UserInfo(user.Id, user.Name);
        }

        public static UserInfoDTO ToUserInfoDTO(this Domain.User user)
        {
            return new UserInfoDTO(userId: user.Id, userName: user.Name);
        }

        public static UserInfoDTO ToUserInfoDTO(this UserEntity entity)
        {
            return new UserInfoDTO(entity.Id, entity.Name);
        }

        public static UserInfo ToUserInfo(this UserEntity entity)
        {
            return new UserInfo(entity.Id, entity.Name);
        }

        public static UserEntity ToEntity(this Domain.User user)
        {
            return new UserEntity()
            {
                Id = user.Id,
                Name = user.Name,
                Weight = user.Weight,
                Gender = user.Gender,
                ProfileImageUrl = user.ProfileImageUrl,
                PushInfo = user.PushInfo,
                Friends = user.Friends,
                MutedFriendUserIds = user.MutedFriendUserIds,
                MutedByFriendUserIds = user.MutedByFriendUserIds,
                MonitoringInstanceId = user.MonitoringInstanceId,
                CurrentVenue = user.CurrentVenue != null ? new VenueEntity()
                {
                    Id = user.CurrentVenue.Id,
                    Location = user.CurrentVenue.Location,
                    Name = user.CurrentVenue.Name,
                    Distance = user.CurrentVenue.Distance
                } : null,
                Language = user.Language,
                LastOnline = user.LastOnline
            };
        }

        public static UserDTO ToDto(this Domain.User entity)
        {
            return new UserDTO()
            {
                Id = entity.Id,
                Name = entity.Name,
                PushInfo = entity.PushInfo,
                ProfileImageUrl = entity.ProfileImageUrl,
                Gender = entity.Gender,
                Language = entity.Language,
                LastOnline = entity.LastOnline,
                Weight = entity.Weight,
                CurrentVenue = entity.CurrentVenue?.ToDto(),
                Friends = entity.Friends?.Select(f=>f.ToDto()).ToList(),
                MutedFriendUserIds = entity.MutedFriendUserIds
            };
        }

        public static UserInfo ToUserInfo(this UserInfoDTO dto)
        {
            return new UserInfo(userId: dto.UserId, userName: dto.UserName);
        }

        public static UserInfoDTO ToDto(this UserInfo userInfo)
        {
            return new UserInfoDTO(userId: userInfo.UserId, userName: userInfo.UserName);
        }
    }
}
