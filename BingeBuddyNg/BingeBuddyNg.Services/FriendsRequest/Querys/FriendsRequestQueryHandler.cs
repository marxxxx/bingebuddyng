using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.FriendsRequest.Querys
{
    public class FriendsRequestQueryHandler :
        IRequestHandler<GetPendingFriendsRequestsQuery, List<FriendRequestDTO>>,
        IRequestHandler<HasPendingFriendsRequestQuery, bool>
    {
        public FriendsRequestQueryHandler(IFriendRequestRepository friendRequestRepository)
        {
            FriendRequestRepository = friendRequestRepository ?? throw new ArgumentNullException(nameof(friendRequestRepository));
        }

        public IFriendRequestRepository FriendRequestRepository { get; }

        public async Task<List<FriendRequestDTO>> Handle(GetPendingFriendsRequestsQuery request, CancellationToken cancellationToken)
        {
            var result = await FriendRequestRepository.GetFriendRequestsAsync(request.UserId);
            return result;
        }

        public async Task<bool> Handle(HasPendingFriendsRequestQuery request, CancellationToken cancellationToken)
        {
            bool result = await FriendRequestRepository.HasPendingFriendRequestAsync(request.UserId, request.RequestingUserId);
            return result;
        }
    }
}
