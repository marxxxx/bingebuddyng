using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace BingeBuddyNg.Core.User.Persistence
{
    public class FriendRequestEntity : TableEntity
    {
        public DateTime RequestTimestamp { get; set; }

        public string RequestingUserId { get; set; }
        public string RequestingUserName { get; set; }
        public string RequestingUserProfileImageUrl { get; set; }

        public string FriendUserId { get; set; }
        public string FriendUserName { get; set; }
        public string FriendUserProfileImageUrl { get; set; }

        public FriendRequestEntity()
        {
        }

        public FriendRequestEntity(
            string partitionKey, string rowKey,
            UserInfo requestingUser,
            UserInfo friend)
            : base(partitionKey, rowKey)
        {
            if (requestingUser == null)
                throw new ArgumentNullException(nameof(requestingUser));
            if (friend == null)
                throw new ArgumentNullException(nameof(friend));

            RequestTimestamp = DateTime.UtcNow;

            RequestingUserId = requestingUser.UserId;
            RequestingUserName = requestingUser.UserName;

            FriendUserId = friend.UserId;
            FriendUserName = friend.UserName;
        }
    }
}
