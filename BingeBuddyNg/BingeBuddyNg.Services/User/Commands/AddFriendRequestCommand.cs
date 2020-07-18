using System;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Core.User;
using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Shared;
using MediatR;

namespace BingeBuddyNg.Core.FriendsRequest.Commands
{
    public class AddFriendRequestCommand : IRequest
    {
        public string RequestingUserId { get; }

        public string FriendUserId { get; }

        public AddFriendRequestCommand(string requestingUserId, string friendUserId)
        {
            RequestingUserId = requestingUserId ?? throw new ArgumentNullException(nameof(requestingUserId));
            FriendUserId = friendUserId ?? throw new ArgumentNullException(nameof(friendUserId));
        }        
    }

    public class AddFriendRequestCommandHandler : IRequestHandler<AddFriendRequestCommand>
    {
        private readonly IUserRepository userRepository;
        private readonly INotificationService notificationService;
        private readonly ITranslationService translationService;

        public AddFriendRequestCommandHandler(IUserRepository userRepository, INotificationService notificationService, ITranslationService translationService)
        {
            this.userRepository = userRepository;
            this.notificationService = notificationService;
            this.translationService = translationService;
        }

        public async Task<Unit> Handle(AddFriendRequestCommand request, CancellationToken cancellationToken)
        {
            var requestingUser = await userRepository.GetUserAsync(request.RequestingUserId);
            var friendUser = await userRepository.GetUserAsync(request.FriendUserId);

            var incomingResult = friendUser.AddFriendRequest(new User.Domain.FriendRequest(DateTime.UtcNow, requestingUser.ToUserInfo(), User.Domain.FriendRequestDirection.Incoming));
            if(incomingResult.IsFailure)
            {
                throw new FriendshipException(incomingResult.Error);
            }

            var outgoingResult = requestingUser.AddFriendRequest(new User.Domain.FriendRequest(DateTime.UtcNow, friendUser.ToUserInfo(), User.Domain.FriendRequestDirection.Outgoing));
            if(outgoingResult.IsFailure)
            {
                throw new FriendshipException(outgoingResult.Error);
            }

            await this.userRepository.UpdateUserAsync(requestingUser.ToEntity());
            await this.userRepository.UpdateUserAsync(friendUser.ToEntity());

            // send push notification to inform user about friend request
            if (friendUser.PushInfo != null)
            {
                var subject = await translationService.GetTranslationAsync(friendUser.Language, "FriendsRequest");
                var message = await translationService.GetTranslationAsync(friendUser.Language, "SentFriendsRequest", requestingUser.Name);

                notificationService.SendWebPushMessage(
                    new[] { friendUser.PushInfo },
                    new WebPushNotificationMessage(Constants.Urls.ApplicationIconUrl,
                    Constants.Urls.ApplicationIconUrl,
                    Constants.Urls.FriendRequestApplicationUrl,
                    subject,
                    message));
            }

            return Unit.Value;
        }
    }
}
