using System;

namespace BingeBuddyNg.Core.User.DTO
{
    public class UserInfoDTO
    {
        public string UserId { get; }

        public string UserName { get; }

        public bool Muted { get; }
                
        public UserInfoDTO(string userId, string userName, bool muted = false)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            Muted = muted;
        }

        public override string ToString()
        {
            return $"{{{nameof(UserId)}={UserId}, {nameof(UserName)}={UserName}, {nameof(Muted)}={Muted.ToString()}}}";
        }
    }
}
