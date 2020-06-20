using BingeBuddyNg.Services.FriendsRequest;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Core.FriendsRequest.Querys
{
    public class GetPendingFriendsRequestsQuery : IRequest<List<FriendRequestDTO>>
    {
        public GetPendingFriendsRequestsQuery(string userId)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        }

        public string UserId { get; }

        public override string ToString()
        {
            return $"{{{nameof(UserId)}={UserId}}}";
        }
    }

    public class GetPendingFriendsRequestsQueryHandler : IRequestHandler<GetPendingFriendsRequestsQuery, List<FriendRequestDTO>>
    {
        private readonly IFriendRequestRepository friendRequestRepository;

        public GetPendingFriendsRequestsQueryHandler(IFriendRequestRepository friendRequestRepository)
        {
            this.friendRequestRepository = friendRequestRepository ?? throw new ArgumentNullException(nameof(friendRequestRepository));
        }

        public async Task<List<FriendRequestDTO>> Handle(GetPendingFriendsRequestsQuery request, CancellationToken cancellationToken)
        {
            var result = await friendRequestRepository.GetFriendRequestsAsync(request.UserId);
            return result;
        }
    }
}
