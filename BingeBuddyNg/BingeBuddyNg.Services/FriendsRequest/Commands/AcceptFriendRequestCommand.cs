using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.User;
using BingeBuddyNg.Shared;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

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

    public class AcceptFriendRequestCommandHandler : IRequestHandler<AcceptFriendRequestCommand>
    {
        public IFriendRequestRepository friendRequestRepository;
        public IUserRepository userRepository;
        public INotificationService notificationService;
        public ITranslationService translationService;

        public AcceptFriendRequestCommandHandler(IFriendRequestRepository friendRequestRepository, IUserRepository userRepository, INotificationService notificationService, ITranslationService translationService)
        {
            this.friendRequestRepository = friendRequestRepository ?? throw new ArgumentNullException(nameof(friendRequestRepository));
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            this.translationService = translationService ?? throw new ArgumentNullException(nameof(translationService));
        }

        public async Task<Unit> Handle(AcceptFriendRequestCommand request, CancellationToken cancellationToken)
        {
            await userRepository.AddFriendAsync(request.AcceptingUserId, request.RequestingUserId);

            await friendRequestRepository.DeleteFriendRequestAsync(request.AcceptingUserId, request.RequestingUserId);

            var acceptingUser = await userRepository.FindUserAsync(request.AcceptingUserId);
            var requestingUser = await userRepository.FindUserAsync(request.RequestingUserId);
            if (requestingUser.PushInfo != null)
            {
                var subject = await translationService.GetTranslationAsync(requestingUser.Language, "FriendsRequest");
                var message = await translationService.GetTranslationAsync(requestingUser.Language, "AcceptedFriendsRequest", acceptingUser.Name);

                notificationService.SendWebPushMessage(
                    new[] { requestingUser.PushInfo },
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
