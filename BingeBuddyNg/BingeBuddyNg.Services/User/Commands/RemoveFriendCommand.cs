using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.User.Commands
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
}
