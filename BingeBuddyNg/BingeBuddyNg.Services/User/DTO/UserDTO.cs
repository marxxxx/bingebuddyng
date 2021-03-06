﻿using System;
using System.Collections.Generic;
using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Core.Statistics.DTO;
using BingeBuddyNg.Core.User.Domain;
using BingeBuddyNg.Core.Venue.DTO;

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

        public List<UserInfoDTO> Friends { get; set; }

        public VenueDTO CurrentVenue { get; set; }

        public string Language { get; set; }

        public UserStatisticsDTO CurrentStats { get; internal set; }

        public DateTime LastOnline { get; set; }

        public List<FriendRequestDTO> IncomingFriendRequests { get; set; }

        public List<FriendRequestDTO> OutgoingFriendRequests { get; set; }
    }
}