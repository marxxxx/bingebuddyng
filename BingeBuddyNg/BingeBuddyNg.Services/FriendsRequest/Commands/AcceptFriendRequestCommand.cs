using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.FriendsRequest.Commands
{
    public class AcceptFriendRequestCommand : IRequest
    {
        public AcceptFriendRequestCommand(string acceptingUserId, string requestingUserId)
        {
            AcceptingUserId = acceptingUserId ?? throw new ArgumentNullException(nameof(acceptingUserId));
            RequestingUserId = requestingUserId ?? throw new ArgumentNullException(nameof(requestingUserId));
        }

        public string AcceptingUserId { get; }
        public string RequestingUserId { get; }
    }
}
