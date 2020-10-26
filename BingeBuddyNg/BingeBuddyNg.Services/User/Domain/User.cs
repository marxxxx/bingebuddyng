using System;
using System.Collections.Generic;
using System.Linq;
using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Core.User.Persistence;
using BingeBuddyNg.Shared;
using CSharpFunctionalExtensions;

namespace BingeBuddyNg.Core.User.Domain
{
    public class User
    {
        private const int MaxCurrentInvitations = 10;

        public string Id { get; }
        public string Name { get; }
        public int? Weight { get; }
        public Gender Gender { get; }
        public string ProfileImageUrl { get; }
        public PushInfo PushInfo { get; private set; }

        private List<UserInfo> _friends = new List<UserInfo>();
        public IReadOnlyList<UserInfo> Friends => _friends.AsReadOnly();

        public Venue.Venue CurrentVenue { get; private set; }
        public string Language { get; private set; }
        public DateTime LastOnline { get; private set; }

        private List<FriendRequest> _pendingFriendRequests = new List<FriendRequest>();
        public IReadOnlyList<FriendRequest> PendingFriendRequests => _pendingFriendRequests.AsReadOnly();

        private List<Guid> _invitations = new List<Guid>();
        public IReadOnlyList<Guid> Invitations => _invitations.AsReadOnly();

        public User(string id, string name, int? weight, Gender gender, string profileImageUrl, PushInfo pushInfo, List<UserInfo> friends, Venue.Venue currentVenue, string language, DateTime lastOnline, List<FriendRequest> friendRequests, List<Guid> invitations)
        {
            this.Id = id ?? throw new ArgumentNullException(nameof(id));
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Weight = weight;
            this.Gender = gender;
            this.ProfileImageUrl = profileImageUrl;
            this.PushInfo = pushInfo;
            this._friends = friends ?? new List<UserInfo>();
            this.CurrentVenue = currentVenue;
            this.Language = language ?? Constants.DefaultLanguage;
            this.LastOnline = lastOnline;
            this._pendingFriendRequests = friendRequests ?? new List<FriendRequest>();
            this._invitations = invitations ?? new List<Guid>();
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

        public IReadOnlyList<string> GetVisibleFriendUserIds(bool includeMe = true)
        {
            var visibleUserIds = Friends.Where(f => !f.Muted).Select(f => f.UserId).ToList();
            if (includeMe)
            {
                visibleUserIds.Add(this.Id);
            }

            return visibleUserIds.AsReadOnly();
        }

        public Result AcceptFriendRequest(UserInfo user)
        {
            if (!PendingFriendRequests.Any(f => f.User.UserId == user.UserId))
            {
                return Result.Failure($"No pending friend request for {user} found!");
            }

            this._pendingFriendRequests.RemoveAll(p => p.User.UserId == user.UserId);
            AddFriend(user);

            return Result.Ok();
        }

        public Result DeclineFriendRequest(UserInfo user)
        {
            if (!PendingFriendRequests.Any(f => f.User.UserId == user.UserId))
            {
                return Result.Failure($"No pending friend request for {user} found!");
            }

            this._pendingFriendRequests.RemoveAll(p => p.User.UserId == user.UserId);
            return Result.Ok();
        }

        /// <summary>
        /// TODO: This method should not be exposed as public method -> find another way to connect friends by accepting invitations
        /// </summary>
        /// <param name="user"></param>
        public void AddFriend(UserInfo user)
        {
            if (!Friends.Any(f => f.UserId == user.UserId))
            {
                _friends.Add(user);
            }
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
            var friend = this.Friends.FirstOrDefault(f => f.UserId == friendUserId);
            if (friend == null)
            {
                Result.Failure($"Friend with id {friendUserId} was not found!");
            }

            friend.Muted = true;

            return Result.Ok();
        }

        public Result UnmuteFriend(string friendUserId)
        {
            var friend = this.Friends.FirstOrDefault(f => f.UserId == friendUserId);
            if (friend == null)
            {
                Result.Failure($"Friend with id {friendUserId} was not found!");
            }

            friend.Muted = false;

            return Result.Ok();
        }

        public Result AddFriendRequest(FriendRequest friendRequest)
        {
            var existingRequest = PendingFriendRequests.FirstOrDefault(f => f.User.UserId == friendRequest.User.UserId);
            if (existingRequest == null)
            {
                _pendingFriendRequests.Add(friendRequest);
            }
            else if (friendRequest.Direction != existingRequest.Direction)
            {
                AcceptFriendRequest(friendRequest.User);
            }

            return Result.Ok();
        }

        public Guid IssueInvitation()
        {
            var invitationToken = Guid.NewGuid();

            if (this.Invitations.Count > MaxCurrentInvitations - 1)
            {
                this._invitations = new List<Guid>(this.Invitations.Skip(this.Invitations.Count - MaxCurrentInvitations + 1));
                this._invitations.Add(invitationToken);
            }

            return invitationToken;
        }

        public Result AcceptInvitation(Guid invitation, UserInfo user)
        {
            if (!this.Invitations.Contains(invitation))
            {
                return Result.Failure("Invitation token unknown or expired!");
            }

            AddFriend(user);
            return Result.Ok();
        }

        public override string ToString()
        {
            return $"{Id} - {Name}";
        }
    }
}