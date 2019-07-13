﻿using System;
using BingeBuddyNg.Services.User;

namespace BingeBuddyNg.Services.Invitation
{
    public class InvitationDTO
    {
        public InvitationDTO(string invitationToken, string invitingUserId, UserInfoDTO invitingUser)
        {
            InvitationToken = invitationToken ?? throw new ArgumentNullException(nameof(invitationToken));
            InvitingUserId = invitingUserId ?? throw new ArgumentNullException(nameof(invitingUserId));
            InvitingUser = invitingUser ?? throw new ArgumentNullException(nameof(invitingUser));
        }

        public string InvitationToken { get; set; }
        public string InvitingUserId { get; set; }
        public UserInfoDTO InvitingUser { get; set; }
    }
}
