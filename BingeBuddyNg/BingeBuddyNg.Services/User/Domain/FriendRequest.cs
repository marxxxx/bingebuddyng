using BingeBuddyNg.Core.User.Persistence;
using System;

namespace BingeBuddyNg.Core.User.Domain
{
    public class FriendRequest
    {
        public UserInfo User { get; }

        public FriendRequestDirection Direction { get; }

        public FriendRequest(UserInfo user, FriendRequestDirection direction)
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
