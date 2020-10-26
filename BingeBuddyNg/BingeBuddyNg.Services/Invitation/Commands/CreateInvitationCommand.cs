using System;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Core.User;
using MediatR;

namespace BingeBuddyNg.Core.Invitation.Commands
{
    public class CreateInvitationCommand : IRequest<Guid>
    {
        public string UserId { get; }

        public CreateInvitationCommand(string userId)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        }
    }

    public class CreateInvitationCommandHandler : IRequestHandler<CreateInvitationCommand, Guid>
    {
        private readonly IInvitationRepository invitationRepository;
        private readonly IUserRepository userRepository;

        public CreateInvitationCommandHandler(
            IInvitationRepository invitationRepository,
            IUserRepository userRepository)
        {
            this.invitationRepository = invitationRepository;
            this.userRepository = userRepository;
        }

        public async Task<Guid> Handle(CreateInvitationCommand request, CancellationToken cancellationToken)
        {
            var invitingUser = await this.userRepository.GetUserAsync(request.UserId);
            var invitationToken = invitingUser.IssueInvitation();
            await invitationRepository.CreateAsync(invitationToken, request.UserId);
            await this.userRepository.UpdateUserAsync(invitingUser.ToEntity());

            return invitationToken;
        }
    }
}