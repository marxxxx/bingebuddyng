using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Core.Invitation.Commands
{
    public class CreateInvitationCommand : IRequest<string>
    {
        public CreateInvitationCommand(string userId)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        }

        public string UserId { get; }
    }

    public class CreateInvitationCommandHandler : IRequestHandler<CreateInvitationCommand, string>
    {
        private readonly IInvitationRepository invitationRepository;

        public CreateInvitationCommandHandler(IInvitationRepository invitationRepository)
        {
            this.invitationRepository = invitationRepository ?? throw new ArgumentNullException(nameof(invitationRepository));
        }        

        public async Task<string> Handle(CreateInvitationCommand request, CancellationToken cancellationToken)
        {
            var invitationToken = await invitationRepository.CreateInvitationAsync(request.UserId);
            return invitationToken;
        }
    }
}
