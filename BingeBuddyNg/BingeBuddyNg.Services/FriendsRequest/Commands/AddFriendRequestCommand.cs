using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.FriendsRequest.Commands
{
    public class AddFriendRequestCommand : IRequest
    {
        public AddFriendRequestCommand(string requestingUserId, string friendUserId)
        {
            RequestingUserId = requestingUserId ?? throw new ArgumentNullException(nameof(requestingUserId));
            FriendUserId = friendUserId ?? throw new ArgumentNullException(nameof(friendUserId));
        }

        public string RequestingUserId { get; }
        public string FriendUserId { get; }
    }
}
