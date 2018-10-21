using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Models
{
    public class FriendRequestInfo
    {
        public FriendRequestInfo()
        {
        }

        public FriendRequestInfo(UserInfo requestingUser, UserInfo friendUser)
        {
            RequestingUser = requestingUser ?? throw new ArgumentNullException(nameof(requestingUser));
            FriendUser = friendUser ?? throw new ArgumentNullException(nameof(friendUser));
        }

        public UserInfo RequestingUser { get; set; }
        public UserInfo FriendUser { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(RequestingUser)}={RequestingUser}, {nameof(FriendUser)}={FriendUser}}}";
        }
    }
}
