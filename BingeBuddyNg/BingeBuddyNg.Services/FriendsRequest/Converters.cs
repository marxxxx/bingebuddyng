using BingeBuddyNg.Core.FriendsRequest.DTO;
using BingeBuddyNg.Core.FriendsRequest.Persistence;
using BingeBuddyNg.Core.User.DTO;

namespace BingeBuddyNg.Core.FriendsRequest
{
    public static class Converters
    {
        public static FriendRequestDTO ToDto(this FriendRequestEntity entity)
        {
            return new FriendRequestDTO(
                new UserInfoDTO(entity.RequestingUserId, entity.RequestingUserName),
                new UserInfoDTO(entity.FriendUserId, entity.FriendUserName));
        }
    }
}
