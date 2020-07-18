using BingeBuddyNg.Core.User.Domain;
using System;

namespace BingeBuddyNg.Core.User.DTO
{
    public class FriendRequestDTO
    {
        public DateTime RequestTimestamp { get; set; }

        public UserInfoDTO User { get; set; }

        public FriendRequestDirection Direction { get; set; }

        public FriendRequestDTO(DateTime requestTimestamp, UserInfoDTO user, FriendRequestDirection direction)
        {
            RequestTimestamp = requestTimestamp;
            User = user ?? throw new ArgumentNullException(nameof(user));
            Direction = direction;
        }

        public override string ToString()
        {
            return $"{{{nameof(RequestTimestamp)}={RequestTimestamp.ToString()}, {nameof(User)}={User}, {nameof(Direction)}={Direction.ToString()}}}";
        }
    }
}
