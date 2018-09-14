using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BingeBuddyNg.Services.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int? Weight { get; set; }
        public Gender Gender { get; set; }
        public string ProfileImageUrl { get; set; }
        public PushInfo PushInfo { get; set; }
        public List<UserInfo> Friends { get; set; } = new List<UserInfo>();

        public User()
        {

        }

        public UserInfo ToUserInfo()
        {
            return new UserInfo(Id, Name, ProfileImageUrl);
        }

        public void AddFriend(UserInfo user)
        {
            if(Friends.Any(f=>f.UserId == user.UserId) == false)
            {
                Friends.Add(user);
            }
        }

        public void RemoveFriend(UserInfo user)
        {
            var foundFriend = Friends.FirstOrDefault(f => f.UserId == user.UserId);
            if(foundFriend != null)
            {
                Friends.Remove(foundFriend);
            }
        }

        public override string ToString()
        {
            return $"{Id} - {Name}";
        }

    }
}
