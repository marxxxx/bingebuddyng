using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.User.Commands
{
    public class SetFriendMuteStateCommand : IRequest
    {
        public SetFriendMuteStateCommand(string userId, string friendUserId, bool muteState)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            FriendUserId = friendUserId ?? throw new ArgumentNullException(nameof(friendUserId));
            MuteState = muteState;
        }

        public string UserId { get; }
        public string FriendUserId { get; }
        public bool MuteState { get; }
    }
}
