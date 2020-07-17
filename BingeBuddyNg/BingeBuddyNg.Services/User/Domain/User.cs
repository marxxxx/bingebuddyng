using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Core.User.Persistence;
using BingeBuddyNg.Shared;
using CSharpFunctionalExtensions;
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
        public int? Weight { get; }
        public Gender Gender { get; }
        public string ProfileImageUrl { get; }
        public PushInfo PushInfo { get; private set; }

        private List<UserInfo> _friends = new List<UserInfo>();
        public IReadOnlyList<UserInfo> Friends => _friends.AsReadOnly();

        private List<string> _mutedFriendUserIds = new List<string>();
        public IReadOnlyList<string> MutedFriendUserIds => _mutedFriendUserIds.AsReadOnly();

        //public string MonitoringInstanceId { get; private set; }
        public Venue.Venue CurrentVenue { get; private set; }
        public string Language { get; private set; }
        public DateTime LastOnline { get; private set; }

        private List<FriendRequest> _pendingFriendRequests = new List<FriendRequest>();
        public IReadOnlyList<FriendRequest> PendingFriendRequests => _pendingFriendRequests.AsReadOnly();

        public User(string id, string name, int? weight, Gender gender, string profileImageUrl, PushInfo pushInfo, List<UserInfo> friends, List<string> mutedFriendUserIds, Venue.Venue currentVenue, string language, DateTime lastOnline)
        {
            this.Id = id ?? throw new ArgumentNullException(nameof(id));
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Weight = weight;
            this.Gender = gender;
            this.ProfileImageUrl = profileImageUrl;
            this.PushInfo = pushInfo;
            this._friends = friends ?? new List<UserInfo>();
            this._mutedFriendUserIds = mutedFriendUserIds ?? new List<string>();
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
            var visibleUserIds = Friends.Select(f => f.UserId).Except(MutedFriendUserIds);
            if (includeMe)
            {
                visibleUserIds = visibleUserIds.Union(new[] { this.Id, BingeBuddyUser.Id });
            }
            return visibleUserIds.ToList();
        }

        public Result AcceptFriendRequest(UserInfo user)
        {
            if(!PendingFriendRequests.Any(f=>f.User == user))
            {
                return Result.Failure($"No pending friend request for {user} found!");
            }

            this._pendingFriendRequests.RemoveAll(p => p.User == user);

            if (!Friends.Any(f => f.UserId == user.UserId))
            {
                _friends.Add(user);
            }

            return Result.Ok();
        }

        public Result DeclineFriendRequest(UserInfo user)
        {
            if (!PendingFriendRequests.Any(f => f.User == user))
            {
                return Result.Failure($"No pending friend request for {user} found!");
            }

            this._pendingFriendRequests.RemoveAll(p => p.User == user);
            return Result.Ok();
        }

        public void RemoveFriend(string userId)
        {
            var foundFriend = Friends.FirstOrDefault(f => f.UserId == userId);
            if (foundFriend != null)
            {
                _friends.Remove(foundFriend);
            }
        }

        public Result MuteFriend(string friendUserId)
        {
            if (!this.Friends.Any(f => f.UserId == friendUserId))
            {
                Result.Failure($"Friend with id {friendUserId} was not found!");
            }

            if (!this.MutedFriendUserIds.Contains(friendUserId))
            {
                this._mutedFriendUserIds.Add(friendUserId);
            }

            return Result.Ok();
        }

        public Result UnmuteFriend(string friendUserId)
        {
            if (!this.Friends.Any(f => f.UserId == friendUserId))
            {
                Result.Failure($"Friend with id {friendUserId} was not found!");
            }

            if (this.MutedFriendUserIds.Contains(friendUserId))
            {
                this._mutedFriendUserIds.Remove(friendUserId);
            }

            return Result.Ok();
        }

        public Result AddFriendRequest(FriendRequest friendRequest)
        {
            var existingRequest = PendingFriendRequests.FirstOrDefault(f => f.User == friendRequest.User);
            if(existingRequest == null)
            {
                _pendingFriendRequests.Add(friendRequest);
            }
            else if (friendRequest.Direction != existingRequest.Direction)
            {
                AcceptFriendRequest(friendRequest.User);
            }

            return Result.Ok();
        }

        //public void UpdateMonitoringInstance(string instanceId)
        //{
        //    this.MonitoringInstanceId = instanceId;
        //}

        public override string ToString()
        {
            return $"{Id} - {Name}";
        }
    }
}
