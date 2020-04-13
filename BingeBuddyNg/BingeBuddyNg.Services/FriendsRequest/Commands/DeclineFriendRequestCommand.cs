using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.FriendsRequest.Commands
{
    public class DeclineFriendRequestCommand : IRequest
    {
        public DeclineFriendRequestCommand(string decliningUserId, string requestingUserId)
        {
            DecliningUserId = decliningUserId ?? throw new ArgumentNullException(nameof(decliningUserId));
            RequestingUserId = requestingUserId ?? throw new ArgumentNullException(nameof(requestingUserId));
        }

        public string DecliningUserId { get; }
        public string RequestingUserId { get; }
    }

    public class DeclineFriendRequestCommandHandler : IRequestHandler<DeclineFriendRequestCommand>
    {
        private readonly IFriendRequestRepository friendRequestRepository;

        public DeclineFriendRequestCommandHandler(IFriendRequestRepository friendRequestRepository)
        {
            this.friendRequestRepository = friendRequestRepository ?? throw new ArgumentNullException(nameof(friendRequestRepository));
        }

        public async Task<Unit> Handle(DeclineFriendRequestCommand request, CancellationToken cancellationToken)
        {
            await friendRequestRepository.DeleteFriendRequestAsync(request.DecliningUserId, request.RequestingUserId);
            return Unit.Value;
        }
    }
}
