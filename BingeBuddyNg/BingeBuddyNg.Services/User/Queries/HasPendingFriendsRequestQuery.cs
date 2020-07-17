using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Core.User.Querys
{
    public class HasPendingFriendsRequestQuery : IRequest<bool>
    {
        public HasPendingFriendsRequestQuery(string userId, string requestingUserId)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            RequestingUserId = requestingUserId ?? throw new ArgumentNullException(nameof(requestingUserId));
        }

        public string UserId { get; }
        public string RequestingUserId { get; }
    }

    public class HasPendingFriendsRequestQueryHandler : IRequestHandler<HasPendingFriendsRequestQuery, bool>
    {
        private readonly IFriendRequestRepository friendRequestRepository;

        public HasPendingFriendsRequestQueryHandler(IFriendRequestRepository friendRequestRepository)
        {
            this.friendRequestRepository = friendRequestRepository ?? throw new ArgumentNullException(nameof(friendRequestRepository));
        }               

        public async Task<bool> Handle(HasPendingFriendsRequestQuery request, CancellationToken cancellationToken)
        {
            bool result = await friendRequestRepository.HasPendingFriendRequestAsync(request.UserId, request.RequestingUserId);
            return result;
        }
    }
}
