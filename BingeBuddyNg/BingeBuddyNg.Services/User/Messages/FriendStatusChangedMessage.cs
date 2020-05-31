using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.User.Messages
{
    public enum FriendStatus
    {
        Unknown,
        Added,
        Removed
    }

    public class FriendStatusChangedMessage
    {
        public FriendStatusChangedMessage(FriendStatus status, string userId, string friendUserId)
        {
            this.Status = status;
            this.UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            this.FriendUserId = friendUserId ?? throw new ArgumentNullException(nameof(friendUserId));
        }

        public FriendStatus Status { get; set; }

        public string UserId { get; set; }

        public string FriendUserId { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(Status)}={Status.ToString()}, {nameof(UserId)}={UserId}, {nameof(FriendUserId)}={FriendUserId}}}";
        }
    }
}
