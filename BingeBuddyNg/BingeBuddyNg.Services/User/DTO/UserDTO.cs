﻿using System;
using System.Collections.Generic;
using BingeBuddyNg.Core.User.Domain;
using BingeBuddyNg.Core.Venue.DTO;
using BingeBuddyNg.Services.Infrastructure;

namespace BingeBuddyNg.Core.User.DTO
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
        public VenueDTO CurrentVenue { get; set; }
        public string Language { get; set; }
        public DateTime LastOnline { get; set; }
        public List<string> MutedFriendUserIds { get; set; }
    }
}
