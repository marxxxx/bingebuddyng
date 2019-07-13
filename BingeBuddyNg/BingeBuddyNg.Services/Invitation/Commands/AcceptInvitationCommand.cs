using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Invitation.Commands
{
    public class AcceptInvitationCommand : IRequest
    {
        public AcceptInvitationCommand(string acceptingUserId, string invitationToken)
        {
            AcceptingUserId = acceptingUserId ?? throw new ArgumentNullException(nameof(acceptingUserId));
            InvitationToken = invitationToken ?? throw new ArgumentNullException(nameof(invitationToken));
        }

        public string AcceptingUserId { get; }
        public string InvitationToken { get; }
    }
}
