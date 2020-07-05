using System;

namespace BingeBuddyNg.Core.User.DTO
{
    public class UserInfoDTO
    {
        public UserInfoDTO()
        {
        }

        public UserInfoDTO(string userId, string userName)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
        }

        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
