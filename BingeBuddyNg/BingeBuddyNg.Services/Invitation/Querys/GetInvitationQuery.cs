using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Invitation.Querys
{
    public class GetInvitationQuery : IRequest<InvitationDTO>
    {
        public GetInvitationQuery(string invitationToken)
        {
            InvitationToken = invitationToken ?? throw new ArgumentNullException(nameof(invitationToken));
        }

        public string InvitationToken { get; }
    }
}
