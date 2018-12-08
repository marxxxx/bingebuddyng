using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Models
{
    public class InvitationInfo
    {
        public InvitationInfo(string invitationToken, UserInfo invitingUser)
        {
            InvitationToken = invitationToken ?? throw new ArgumentNullException(nameof(invitationToken));
            InvitingUser = invitingUser ?? throw new ArgumentNullException(nameof(invitingUser));
        }

        public string InvitationToken { get; set; }
        public UserInfo InvitingUser { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(InvitationToken)}={InvitationToken}, {nameof(InvitingUser)}={InvitingUser}}}";
        }
    }
}
