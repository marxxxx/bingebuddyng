using System;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Core.User;
using MediatR;

namespace BingeBuddyNg.Core.FriendsRequest.Commands
{
    public class DeclineFriendRequestCommand : IRequest
    {
        public string DecliningUserId { get; }

        public string RequestingUserId { get; }

        public DeclineFriendRequestCommand(string decliningUserId, string requestingUserId)
        {
            DecliningUserId = decliningUserId ?? throw new ArgumentNullException(nameof(decliningUserId));
            RequestingUserId = requestingUserId ?? throw new ArgumentNullException(nameof(requestingUserId));
        }
    }

    public class DeclineFriendRequestCommandHandler : IRequestHandler<DeclineFriendRequestCommand>
    {
        private readonly IUserRepository userRepository;

        public DeclineFriendRequestCommandHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<Unit> Handle(DeclineFriendRequestCommand request, CancellationToken cancellationToken)
        {
            var decliningUser = await userRepository.GetUserAsync(request.DecliningUserId);
            var requestingUser = await userRepository.GetUserAsync(request.RequestingUserId);

            decliningUser.DeclineFriendRequest(requestingUser.ToUserInfo());
            requestingUser.DeclineFriendRequest(decliningUser.ToUserInfo());

            await this.userRepository.UpdateUserAsync(decliningUser.ToEntity());
            await this.userRepository.UpdateUserAsync(requestingUser.ToEntity());

            return Unit.Value;
        }
    }
}