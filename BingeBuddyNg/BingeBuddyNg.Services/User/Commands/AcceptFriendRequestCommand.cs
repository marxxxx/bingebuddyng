using System;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Core.User;
using BingeBuddyNg.Core.User.Messages;
using BingeBuddyNg.Shared;
using MediatR;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Core.FriendsRequest.Commands
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
        private readonly IUserRepository userRepository;
        private readonly INotificationService notificationService;
        private readonly ITranslationService translationService;
        private readonly IStorageAccessService storageAccessService;

        public AcceptFriendRequestCommandHandler(
            IUserRepository userRepository,
            INotificationService notificationService,
            ITranslationService translationService,
            IStorageAccessService storageAccessService)
        {
            this.userRepository = userRepository;
            this.notificationService = notificationService;
            this.translationService = translationService;
            this.storageAccessService = storageAccessService;
        }

        public async Task<Unit> Handle(AcceptFriendRequestCommand request, CancellationToken cancellationToken)
        {
            var acceptingUser = await userRepository.GetUserAsync(request.AcceptingUserId);
            var requestingUser = await userRepository.GetUserAsync(request.RequestingUserId);

            var acceptanceResult = acceptingUser.AcceptFriendRequest(requestingUser.ToUserInfo());
            if (acceptanceResult.IsFailure)
            {
                throw new FriendshipException(acceptanceResult.Error);
            }

            var requestingResult = requestingUser.AcceptFriendRequest(acceptingUser.ToUserInfo());
            if (requestingResult.IsFailure)
            {
                throw new FriendshipException(acceptanceResult.Error);
            }

            await userRepository.UpdateUserAsync(acceptingUser.ToEntity());
            await userRepository.UpdateUserAsync(requestingUser.ToEntity());

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

            await this.storageAccessService.AddQueueMessage(QueueNames.FriendStatusChanged, new FriendStatusChangedMessage(FriendStatus.Added, request.AcceptingUserId, request.RequestingUserId));

            return Unit.Value;
        }
    }
}