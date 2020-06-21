﻿using System;
using BingeBuddyNg.Core.User.DTO;

namespace BingeBuddyNg.Core.FriendsRequest.DTO
{
    public class FriendRequestDTO
    {
        public FriendRequestDTO()
        {
        }

        public FriendRequestDTO(UserInfoDTO requestingUser, UserInfoDTO friendUser)
        {
            RequestingUser = requestingUser ?? throw new ArgumentNullException(nameof(requestingUser));
            FriendUser = friendUser ?? throw new ArgumentNullException(nameof(friendUser));
        }

        public UserInfoDTO RequestingUser { get; set; }
        public UserInfoDTO FriendUser { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(RequestingUser)}={RequestingUser}, {nameof(FriendUser)}={FriendUser}}}";
        }
    }
}
