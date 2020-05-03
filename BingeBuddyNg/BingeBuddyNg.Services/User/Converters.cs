using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.User
{
    public static class Converters
    {
        public static UserInfo ToUserInfo(this User user)
        {
            return new UserInfo(user.Id, user.Name);
        }

        public static UserInfoDTO ToUserInfoDTO(this User user)
        {
            return new UserInfoDTO(userId: user.Id, userName: user.Name);
        }
    }
}
