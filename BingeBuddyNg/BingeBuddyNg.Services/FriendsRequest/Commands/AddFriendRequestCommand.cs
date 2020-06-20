using System;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Core.User;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Shared;
using MediatR;

namespace BingeBuddyNg.Core.FriendsRequest.Commands
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

    public class AddFriendRequestCommandHandler : IRequestHandler<AddFriendRequestCommand>
    {
        private readonly IFriendRequestRepository friendRequestRepository;
        private readonly IUserRepository userRepository;
        private readonly INotificationService notificationService;
        private readonly ITranslationService translationService;

        public AddFriendRequestCommandHandler(IFriendRequestRepository friendRequestRepository, IUserRepository userRepository, INotificationService notificationService, ITranslationService translationService)
        {
            this.friendRequestRepository = friendRequestRepository ?? throw new ArgumentNullException(nameof(friendRequestRepository));
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            this.translationService = translationService ?? throw new ArgumentNullException(nameof(translationService));
        }

        public async Task<Unit> Handle(AddFriendRequestCommand request, CancellationToken cancellationToken)
        {
            var hasPendingRequest = await friendRequestRepository.HasPendingFriendRequestAsync(request.FriendUserId, request.RequestingUserId);
            if (hasPendingRequest == false)
            {
                var requestingUser = await userRepository.GetUserAsync(request.RequestingUserId);
                var friendUser = await userRepository.GetUserAsync(request.FriendUserId);

                await friendRequestRepository.AddFriendRequestAsync(friendUser.ToUserInfo(), requestingUser.ToUserInfo());

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
            }

            return Unit.Value;
        }
    }
}
