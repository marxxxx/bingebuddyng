using BingeBuddyNg.Services.User.Persistence;

namespace BingeBuddyNg.Services.User
{
    public static class Converters
    {
        public static UserInfo ToUserInfo(this User user)
        {
            return new UserInfo(user.Id, user.Name);
        }

        public static UserInfoDTO ToUserInfoDTO(this User user)
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

        public static UserEntity ToEntity(this User user)
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
                CurrentVenue = user.CurrentVenue != null ? new Venue.Persistence.VenueEntity()
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

        public static UserDTO ToDto(this User entity)
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
                CurrentVenue = entity.CurrentVenue,
                Friends = entity.Friends,
                MutedFriendUserIds = entity.MutedFriendUserIds
            };
        }
    }
}
