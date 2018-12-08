using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Models
{
    public class Invitation
    {
        public Invitation(string invitationToken, string invitingUserId, string acceptingUserId)
        {
            InvitationToken = invitationToken ?? throw new ArgumentNullException(nameof(invitationToken));
            InvitingUserId = invitingUserId ?? throw new ArgumentNullException(nameof(invitingUserId));
            AcceptingUserId = acceptingUserId;
        }

        public string InvitationToken { get; set; }
        public string InvitingUserId { get; set; }
        public string AcceptingUserId { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(InvitationToken)}={InvitationToken}, {nameof(InvitingUserId)}={InvitingUserId}, {nameof(AcceptingUserId)}={AcceptingUserId}}}";
        }
    }
}
