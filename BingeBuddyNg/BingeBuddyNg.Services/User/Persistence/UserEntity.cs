using System;
using System.Collections.Generic;
using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Core.User.Domain;
using BingeBuddyNg.Core.Venue.Persistence;

namespace BingeBuddyNg.Core.User.Persistence
{
    public class UserEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int? Weight { get; set; }
        public Gender Gender { get; set; }
        public string ProfileImageUrl { get; set; }
        public PushInfo PushInfo { get; set; }
        public List<UserInfo> Friends { get; set; } = new List<UserInfo>();
        public List<FriendRequest> PendingFriendRequests = new List<FriendRequest>();
        public string MonitoringInstanceId { get; set; }
        public VenueEntity CurrentVenue { get; set; }
        public string Language { get; set; }
        public DateTime LastOnline { get; set; }
        public List<Guid> Invitations { get; set; }
    }
}