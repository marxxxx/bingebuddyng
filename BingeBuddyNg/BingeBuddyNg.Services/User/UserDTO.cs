using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.Venue;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.User
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int? Weight { get; set; }
        public Gender Gender { get; set; }
        public string ProfileImageUrl { get; set; }
        public PushInfo PushInfo { get; set; }
        public List<UserInfo> Friends { get; set; }
        public VenueModel CurrentVenue { get; set; }
        public string Language { get; set; }
        public DateTime LastOnline { get; set; }
        public List<string> MutedFriendUserIds { get; set; }
    }
}
