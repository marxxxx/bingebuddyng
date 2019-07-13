using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.FriendsRequest.Querys
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
}
