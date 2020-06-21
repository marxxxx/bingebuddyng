using System;
using BingeBuddyNg.Core.User.DTO;
using BingeBuddyNg.Services.User;

namespace BingeBuddyNg.Core.User
{
    public class UserInfo : IEquatable<UserInfo>
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public UserInfo()
        {
        }

        public UserInfo(string userId, string userName)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
        }

        public bool Equals(UserInfo other)
        {
            if (other == null)
                return false;

            return other.UserId == this.UserId;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as UserInfo);
        }

        public override int GetHashCode()
        {
            if (this.UserId == null)
                return 0;
            return this.UserId.GetHashCode();
        }

        public override string ToString()
        {
            return $"{{{nameof(UserId)}={UserId}, {nameof(UserName)}={UserName}}}";
        }

        internal UserInfoDTO ToUserInfoDTO()
        {
            throw new NotImplementedException();
        }
    }
}
