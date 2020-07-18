using BingeBuddyNg.Core.User.Persistence;
using System;

namespace BingeBuddyNg.Core.User.Domain
{
    public class FriendRequest
    {
        public DateTime RequestTimestamp { get; }

        public UserInfo User { get; }

        public FriendRequestDirection Direction { get; }

        public FriendRequest(DateTime requestTimestamp, UserInfo user, FriendRequestDirection direction)
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
