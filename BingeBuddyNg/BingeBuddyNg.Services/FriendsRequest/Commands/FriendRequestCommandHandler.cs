using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.User;
using BingeBuddyNg.Shared;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.FriendsRequest.Commands
{
    public class FriendRequestCommandHandler :
        IRequestHandler<AcceptFriendRequestCommand>,
        IRequestHandler<AddFriendRequestCommand>,
        IRequestHandler<DeclineFriendRequestCommand>
    {
        public IFriendRequestRepository FriendRequestRepository { get; }
        public IUserRepository UserRepository { get; }
        public INotificationService NotificationService { get; }
        public ITranslationService TranslationService { get; }

        public FriendRequestCommandHandler(IFriendRequestRepository friendRequestRepository, IUserRepository userRepository, INotificationService notificationService, ITranslationService translationService)
        {
            FriendRequestRepository = friendRequestRepository ?? throw new ArgumentNullException(nameof(friendRequestRepository));
            UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            NotificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            TranslationService = translationService ?? throw new ArgumentNullException(nameof(translationService));
        }

        public async Task<Unit> Handle(AcceptFriendRequestCommand request, CancellationToken cancellationToken)
        {
            await UserRepository.AddFriendAsync(request.AcceptingUserId, request.RequestingUserId);

            await FriendRequestRepository.DeleteFriendRequestAsync(request.AcceptingUserId, request.RequestingUserId);

            var acceptingUser = await UserRepository.FindUserAsync(request.AcceptingUserId);
            var requestingUser = await UserRepository.FindUserAsync(request.RequestingUserId);
            if (requestingUser.PushInfo != null)
            {
                var subject = await TranslationService.GetTranslationAsync(requestingUser.Language, "FriendsRequest");
                var message = await TranslationService.GetTranslationAsync(requestingUser.Language, "AcceptedFriendsRequest", acceptingUser.Name);

                NotificationService.SendWebPushMessage(new[] { requestingUser.PushInfo }, new NotificationMessage(Constants.Urls.ApplicationIconUrl,
                    Constants.Urls.ApplicationIconUrl, Constants.Urls.FriendRequestApplicationUrl, subject, message));
            }

            return Unit.Value;
        }

        public async Task<Unit> Handle(AddFriendRequestCommand request, CancellationToken cancellationToken)
        {
            var hasPendingRequest = await FriendRequestRepository.HasPendingFriendRequestAsync(request.FriendUserId, request.RequestingUserId);
            if (hasPendingRequest == false)
            {
                var requestingUser = await UserRepository.FindUserAsync(request.RequestingUserId);
                var friendUser = await UserRepository.FindUserAsync(request.FriendUserId);

                await FriendRequestRepository.AddFriendRequestAsync(friendUser.ToUserInfo(), requestingUser.ToUserInfo());

                // send push notification to inform user about friend request
                if (friendUser.PushInfo != null)
                {
                    var subject = await TranslationService.GetTranslationAsync(friendUser.Language, "FriendsRequest");
                    var message = await TranslationService.GetTranslationAsync(friendUser.Language, "SentFriendsRequest", requestingUser.Name);

                    NotificationService.SendWebPushMessage(new[] { friendUser.PushInfo }, new NotificationMessage(Constants.Urls.ApplicationIconUrl,
                        Constants.Urls.ApplicationIconUrl, Constants.Urls.FriendRequestApplicationUrl, subject, message));
                }
            }

            return Unit.Value;
        }

        public async Task<Unit> Handle(DeclineFriendRequestCommand request, CancellationToken cancellationToken)
        {
            await FriendRequestRepository.DeleteFriendRequestAsync(request.DecliningUserId, request.RequestingUserId);
            return Unit.Value;
        }
    }
}
