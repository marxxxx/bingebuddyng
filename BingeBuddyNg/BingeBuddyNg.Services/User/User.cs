using BingeBuddyNg.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BingeBuddyNg.Services.User
{
    public class User
    {
        public static readonly string BingeBuddyUserId = "bingebuddy";
        public static readonly string BingeBuddyUserName = "Binge Buddy";

        public string Id { get; set; }
        public string Name { get; set; }
        public int? Weight { get; set; }
        public Gender Gender { get; set; }
        public string ProfileImageUrl { get; set; }
        public PushInfo PushInfo { get; set; }
        public List<UserInfo> Friends { get; set; } = new List<UserInfo>();
        public List<string> MutedFriendUserIds { get; set; } = new List<string>();
        public List<string> MutedByFriendUserIds { get; set; } = new List<string>();
        public string MonitoringInstanceId { get; set; }
        public Venue.Venue CurrentVenue { get; set; }
        public string Language { get; set; }
        public DateTime LastOnline { get; set; }

        public User()
        {

        }

        public UserInfo ToUserInfo()
        {
            return new UserInfo(Id, Name);
        }


        public List<string> GetVisibleFriendUserIds(bool includeMe = true)
        {
            var visibleUserIds = Friends.Select(f => f.UserId).Except(MutedByFriendUserIds);
            if(includeMe)
            {
                visibleUserIds = visibleUserIds.Union(new[] { this.Id, BingeBuddyUserId });
            }
            return visibleUserIds.ToList();
        }

        public void AddFriend(UserInfo user)
        {
            if(Friends.Any(f=>f.UserId == user.UserId) == false)
            {
                Friends.Add(user);
            }
        }

        public void RemoveFriend(string userId)
        {
            var foundFriend = Friends.FirstOrDefault(f => f.UserId == userId);
            if(foundFriend != null)
            {
                Friends.Remove(foundFriend);
            }
        }

        public void SetFriendMuteState(string friendUserId, bool mute)
        {
            SetMuteState(this.MutedFriendUserIds, friendUserId, mute);
        }

        public void SetMutedByFriendState(string friendUserId, bool mute)
        {
            SetMuteState(this.MutedByFriendUserIds, friendUserId, mute);
        }

        private void SetMuteState(List<string> userIdList, string friendUserId, bool mute)
        {
            var friend = this.Friends.FirstOrDefault(f => f.UserId == friendUserId);
            if (friend == null)
            {
                throw new NotFoundException($"Friend with id {friendUserId} was not found!");
            }

            if (mute)
            {
                if (userIdList.Contains(friendUserId) == false)
                {
                    userIdList.Add(friendUserId);
                }
            }
            else
            {
                if (userIdList.Contains(friendUserId))
                {
                    userIdList.Remove(friendUserId);
                }
            }
        }

        public override string ToString()
        {
            return $"{Id} - {Name}";
        }

    }
}
