using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.User;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

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

    public class GetInvitationQueryHandler :
        IRequestHandler<GetInvitationQuery, InvitationDTO>
    {
        private readonly IInvitationRepository invitationRepository;
        private readonly IUserRepository userRepository;

        public GetInvitationQueryHandler(IInvitationRepository invitationRepository, IUserRepository userRepository)
        {
            this.invitationRepository = invitationRepository ?? throw new ArgumentNullException(nameof(invitationRepository));
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<InvitationDTO> Handle(GetInvitationQuery request, CancellationToken cancellationToken)
        {
            var invitation = await this.invitationRepository.GetInvitationAsync(request.InvitationToken);
            var user = await this.userRepository.GetUserAsync(invitation.InvitingUserId);

            var result = new InvitationDTO(invitation.InvitationToken, invitation.InvitingUserId, new UserInfoDTO(user.Id, user.Name));
            return result;
        }
    }
}
