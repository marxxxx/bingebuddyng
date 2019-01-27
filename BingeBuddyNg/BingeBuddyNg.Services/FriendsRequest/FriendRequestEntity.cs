using System;
using BingeBuddyNg.Services.User;
using Microsoft.WindowsAzure.Storage.Table;

namespace BingeBuddyNg.Services.FriendsRequest
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

        //public FriendRequestEntity(string userId, string requestingUserId)
        //    : base(userId, requestingUserId)
        //{
        //    this.UserId = userId;
        //    this.RequestingUserId = requestingUserId;
        //}

        public FriendRequestEntity(
            string partitionKey, string rowKey,
            UserInfo requestingUser,
            UserInfo friend)
            :base(partitionKey, rowKey)
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
