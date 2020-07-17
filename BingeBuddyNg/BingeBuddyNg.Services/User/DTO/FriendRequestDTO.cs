using System;

namespace BingeBuddyNg.Core.User.DTO
{
    public class FriendRequestDTO
    {
        public UserInfoDTO User { get; set; }

        public FriendRequestDirection Direction { get; set; }

        public FriendRequestDTO(UserInfoDTO user, FriendRequestDirection direction)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
            Direction = direction;
        }

        public override string ToString()
        {
            return $"{{{nameof(User)}={User}, {nameof(Direction)}={Direction.ToString()}}}";
        }
    }
}
