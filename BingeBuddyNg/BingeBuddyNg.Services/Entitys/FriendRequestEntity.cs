using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Entitys
{
    public class FriendRequestEntity : TableEntity
    {
        public string UserId { get; set; }
        public DateTime RequestTimestamp { get; set; }
        public string RequestingUserId { get; set; }
        public string RequestingUserName { get; set; }
        public string RequestingUserProfileImageUrl { get; set; }
        
        public FriendRequestEntity()
        {
        }

        public FriendRequestEntity(string userId, string requestingUserId)
            : base(userId, requestingUserId)
        {
            this.UserId = userId;
            this.RequestingUserId = requestingUserId;
        }

        public FriendRequestEntity(string userId, DateTime requestTimestamp, string requestingUserId, string requestingUserName, string requestingUserProfileImageUrl)
            :base(userId, requestingUserId)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            RequestTimestamp = requestTimestamp;
            RequestingUserId = requestingUserId ?? throw new ArgumentNullException(nameof(requestingUserId));
            RequestingUserName = requestingUserName ?? throw new ArgumentNullException(nameof(requestingUserName));
            RequestingUserProfileImageUrl = requestingUserProfileImageUrl ?? throw new ArgumentNullException(nameof(requestingUserProfileImageUrl));
        }
    }
}
