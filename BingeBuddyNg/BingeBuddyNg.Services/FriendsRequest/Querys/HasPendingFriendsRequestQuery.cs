using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.FriendsRequest.Querys
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
}
