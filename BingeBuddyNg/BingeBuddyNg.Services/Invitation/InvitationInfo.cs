using System;
using BingeBuddyNg.Services.User;

namespace BingeBuddyNg.Services.Invitation
{
    public class InvitationInfo
    {
        public InvitationInfo(Invitation invitation, UserInfo invitingUser)
        {
            Invitation = invitation ?? throw new ArgumentNullException(nameof(invitation));
            InvitingUser = invitingUser ?? throw new ArgumentNullException(nameof(invitingUser));
        }

        public Invitation Invitation { get; set; }
        public UserInfo InvitingUser { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(Invitation)}={Invitation}, {nameof(InvitingUser)}={InvitingUser}}}";
        }
    }
}
