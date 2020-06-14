using System;
using System.Collections.Generic;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.Venue.Persistence;

namespace BingeBuddyNg.Services.User.Persistence
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
        public List<string> MutedFriendUserIds { get; set; } = new List<string>();
        public List<string> MutedByFriendUserIds { get; set; } = new List<string>();
        public string MonitoringInstanceId { get; set; }
        public VenueEntity CurrentVenue { get; set; }
        public string Language { get; set; }
        public DateTime LastOnline { get; set; }
    }
}
