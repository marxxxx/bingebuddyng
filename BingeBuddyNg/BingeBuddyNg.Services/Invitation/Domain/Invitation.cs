using System;

namespace BingeBuddyNg.Core.Invitation
{
    public class Invitation
    {
        public Invitation(string invitationToken, string invitingUserId)
        {
            InvitationToken = invitationToken ?? throw new ArgumentNullException(nameof(invitationToken));
            InvitingUserId = invitingUserId ?? throw new ArgumentNullException(nameof(invitingUserId));
        }

        public string InvitationToken { get; set; }
        public string InvitingUserId { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(InvitationToken)}={InvitationToken}, {nameof(InvitingUserId)}={InvitingUserId}}}";
        }
    }
}
