using System;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Invitation.DTO;
using BingeBuddyNg.Core.User;
using BingeBuddyNg.Core.User.DTO;
using MediatR;

namespace BingeBuddyNg.Core.Invitation.Querys
{
    public class GetInvitationQuery : IRequest<InvitationDTO>
    {
        public Guid InvitationToken { get; }

        public GetInvitationQuery(Guid invitationToken)
        {
            InvitationToken = invitationToken;
        }        
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
            var invitation = await this.invitationRepository.GetAsync(request.InvitationToken);
            var user = await this.userRepository.GetUserAsync(invitation.InvitingUserId);

            var result = new InvitationDTO(invitation.InvitationToken, invitation.InvitingUserId, new UserInfoDTO(user.Id, user.Name));
            return result;
        }
    }
}
