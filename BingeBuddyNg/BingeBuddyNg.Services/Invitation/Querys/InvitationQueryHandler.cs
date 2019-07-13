using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.User;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Invitation.Querys
{
    public class InvitationQueryHandler :
        IRequestHandler<GetInvitationQuery, InvitationDTO>
    {
        public IInvitationRepository InvitationRepository { get; }
        public IUserRepository UserRepository { get; }

        public InvitationQueryHandler(IInvitationRepository invitationRepository, IUserRepository userRepository)
        {
            InvitationRepository = invitationRepository ?? throw new ArgumentNullException(nameof(invitationRepository));
            UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<InvitationDTO> Handle(GetInvitationQuery request, CancellationToken cancellationToken)
        {
            var invitation = await this.InvitationRepository.GetInvitationAsync(request.InvitationToken);
            var user = await this.UserRepository.FindUserAsync(invitation.InvitingUserId);
            if (user == null)
            {
                throw new NotFoundException($"Inviting user {invitation.InvitingUserId} not found!");
            }

            var result = new InvitationDTO(invitation.InvitationToken, invitation.InvitingUserId, new UserInfoDTO(user.Id, user.Name));
            return result;
        }
    }
}
