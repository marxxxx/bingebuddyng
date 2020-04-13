using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

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

    public class SetFriendMuteStateCommandHandler : IRequestHandler<SetFriendMuteStateCommand>
    {
        private readonly IUserRepository userRepository;

        public SetFriendMuteStateCommandHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }


        public async Task<Unit> Handle(SetFriendMuteStateCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.FindUserAsync(request.UserId);
            user.SetFriendMuteState(request.FriendUserId, request.MuteState);

            var mutedUser = await userRepository.FindUserAsync(request.FriendUserId);
            mutedUser.SetMutedByFriendState(request.UserId, request.MuteState);

            Task.WaitAll(userRepository.UpdateUserAsync(user), userRepository.UpdateUserAsync(mutedUser));

            return Unit.Value;
        }
    }
}
