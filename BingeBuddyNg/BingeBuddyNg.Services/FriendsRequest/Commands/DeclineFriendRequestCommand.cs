using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

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
}
