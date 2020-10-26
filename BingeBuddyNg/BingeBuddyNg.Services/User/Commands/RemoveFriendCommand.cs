using System;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Core.User.Messages;
using MediatR;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Core.User.Commands
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
            var results = await Task.WhenAll(userRepository.GetUserAsync(request.UserId), userRepository.GetUserAsync(request.FriendUserId));

            results[0].RemoveFriend(request.FriendUserId);
            results[1].RemoveFriend(request.UserId);

            await Task.WhenAll(userRepository.UpdateUserAsync(results[0].ToEntity()), userRepository.UpdateUserAsync(results[1].ToEntity()));

            await this.storageAccessService.AddQueueMessage(QueueNames.FriendStatusChanged,
                new FriendStatusChangedMessage(FriendStatus.Removed, request.UserId, request.FriendUserId));
            return Unit.Value;
        }
    }
}