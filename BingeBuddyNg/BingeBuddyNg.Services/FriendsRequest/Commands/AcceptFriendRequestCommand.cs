using BingeBuddyNg.Core.User.Commands;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.User;
using BingeBuddyNg.Services.User.Messages;
using BingeBuddyNg.Shared;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using static BingeBuddyNg.Shared.Constants;

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
        private readonly IFriendRequestRepository friendRequestRepository;
        private readonly IUserRepository userRepository;
        private readonly AddFriendCommand addFriendCommand;
        private readonly INotificationService notificationService;
        private readonly ITranslationService translationService;
        private readonly IStorageAccessService storageAccessService;

        public AcceptFriendRequestCommandHandler(
            IFriendRequestRepository friendRequestRepository, 
            IUserRepository userRepository, 
            AddFriendCommand addFriendCommand,
            INotificationService notificationService, 
            ITranslationService translationService,
            IStorageAccessService storageAccessService)
        {
            this.friendRequestRepository = friendRequestRepository ?? throw new ArgumentNullException(nameof(friendRequestRepository));
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.addFriendCommand = addFriendCommand ?? throw new ArgumentNullException(nameof(addFriendCommand));
            this.notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            this.translationService = translationService ?? throw new ArgumentNullException(nameof(translationService));
            this.storageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
        }

        public async Task<Unit> Handle(AcceptFriendRequestCommand request, CancellationToken cancellationToken)
        {
            await this.addFriendCommand.ExecuteAsync(request.AcceptingUserId, request.RequestingUserId);            

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

            await this.storageAccessService.AddQueueMessage(QueueNames.FriendStatusChanged, new FriendStatusChangedMessage(FriendStatus.Added, request.AcceptingUserId, request.RequestingUserId));

            return Unit.Value;
        }
    }
}
