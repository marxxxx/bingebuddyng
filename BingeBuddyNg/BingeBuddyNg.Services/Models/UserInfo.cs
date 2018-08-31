using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Models
{
    public class UserInfo
    {
        public UserInfo()
        {
        }

        public UserInfo(string userId, string userName, string userProfileImageUrl)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            UserProfileImageUrl = userProfileImageUrl ?? throw new ArgumentNullException(nameof(userProfileImageUrl));
        }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserProfileImageUrl { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(UserId)}={UserId}, {nameof(UserName)}={UserName}, {nameof(UserProfileImageUrl)}={UserProfileImageUrl}}}";
        }
    }
}
