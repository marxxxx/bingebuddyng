using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.User.Messages;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using static BingeBuddyNg.Shared.Constants;

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
        private readonly IStorageAccessService storageAccessService;

        public RemoveFriendCommandHandler(IUserRepository userRepository, IStorageAccessService storageAccessService)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.storageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
        }

        public async Task<Unit> Handle(RemoveFriendCommand request, CancellationToken cancellationToken)
        {
            await this.userRepository.RemoveFriendAsync(request.UserId, request.FriendUserId);
            await this.storageAccessService.AddQueueMessage(QueueNames.FriendStatusChanged, 
                new FriendStatusChangedMessage(FriendStatus.Removed, request.UserId, request.FriendUserId));
            return Unit.Value;
        }
    }
}
