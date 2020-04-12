using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.User.Commands
{
    public class RemoveFriendCommand : IRequest
    {
        public RemoveFriendCommand(string userId, string friendUserId)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            FriendUserId = friendUserId ?? throw new ArgumentNullException(nameof(friendUserId));
        }

        public string UserId { get; }
        public string FriendUserId { get; }
    }

    public class RemoveFriendCommandHandler : IRequestHandler<RemoveFriendCommand>
    {
        private readonly IUserRepository userRepository;

        public RemoveFriendCommandHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<Unit> Handle(RemoveFriendCommand request, CancellationToken cancellationToken)
        {
            await this.userRepository.RemoveFriendAsync(request.UserId, request.FriendUserId);
            return Unit.Value;
        }
    }
}
