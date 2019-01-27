using System;

namespace BingeBuddyNg.Services.User
{
    public class ProfileUpdateMessage
    {
        public ProfileUpdateMessage()
        {
        }

        public ProfileUpdateMessage(string userId, string userProfileImageUrl)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            UserProfileImageUrl = userProfileImageUrl ?? throw new ArgumentNullException(nameof(userProfileImageUrl));
        }

        public string UserId { get; set; }
        public string UserProfileImageUrl { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(UserId)}={UserId}, {nameof(UserProfileImageUrl)}={UserProfileImageUrl}}}";
        }
    }
}
