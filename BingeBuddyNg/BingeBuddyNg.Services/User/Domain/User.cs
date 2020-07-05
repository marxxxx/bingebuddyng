using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Core.User.Persistence;
using BingeBuddyNg.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Core.User.Domain
{
    public class User
    {
        public string Id { get; }
        public string Name { get; }
        public int? Weight { get; private set; }
        public Gender Gender { get; private set; }
        public string ProfileImageUrl { get; private set; }
        public PushInfo PushInfo { get; private set; }
        public List<UserInfo> Friends { get; private set; } = new List<UserInfo>();
        public List<string> MutedFriendUserIds { get; private set; } = new List<string>();

        public List<string> MutedByFriendUserIds { get; private set; } = new List<string>();
        public string MonitoringInstanceId { get; private set; }
        public Venue.Venue CurrentVenue { get; private set; }
        public string Language { get; private set; }
        public DateTime LastOnline { get; private set; }

        public User(string id, string name, int? weight, Gender gender, string profileImageUrl, PushInfo pushInfo, List<UserInfo> friends, List<string> mutedFriendUserIds, List<string> mutedByFriendUserIds, string monitoringInstanceId, Venue.Venue currentVenue, string language, DateTime lastOnline)
        {
            this.Id = id ?? throw new ArgumentNullException(nameof(id));
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Weight = weight;
            this.Gender = gender;
            this.ProfileImageUrl = profileImageUrl;
            this.PushInfo = pushInfo;
            this.Friends = friends ?? new List<UserInfo>();
            this.MutedFriendUserIds = mutedFriendUserIds ?? new List<string>();
            this.MutedByFriendUserIds = mutedByFriendUserIds ?? new List<string>();
            this.MonitoringInstanceId = monitoringInstanceId;
            this.CurrentVenue = currentVenue;
            this.Language = language ?? Constants.DefaultLanguage;
            this.LastOnline = lastOnline;
        }

        public void EnterVenue(Venue.Venue venue)
        {
            if (venue == null)
            {
                throw new ArgumentNullException(nameof(venue));
            }

            this.CurrentVenue = venue;
        }

        public void LeaveVenue()
        {
            this.CurrentVenue = null;
        }

        public List<string> GetVisibleFriendUserIds(bool includeMe = true)
        {
            var visibleUserIds = Friends.Select(f => f.UserId).Except(MutedByFriendUserIds);
            if (includeMe)
            {
                visibleUserIds = visibleUserIds.Union(new[] { this.Id, BingeBuddyUser.Id });
            }
            return visibleUserIds.ToList();
        }

        public void AddFriend(UserInfo user)
        {
            if (Friends.Any(f => f.UserId == user.UserId) == false)
            {
                Friends.Add(user);
            }
        }

        public void RemoveFriend(string userId)
        {
            var foundFriend = Friends.FirstOrDefault(f => f.UserId == userId);
            if (foundFriend != null)
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

        public void UpdateMonitoringInstance(string instanceId)
        {
            this.MonitoringInstanceId = instanceId;
        }

        public override string ToString()
        {
            return $"{Id} - {Name}";
        }
    }
}
