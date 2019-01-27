using System;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.User;
using BingeBuddyNg.Shared;

namespace BingeBuddyNg.Services.FriendsRequest
{
    public class FriendRequestService : IFriendRequestService
    {
        public IFriendRequestRepository FriendRequestRepository { get; }
        public IUserRepository UserRepository { get; }
        public INotificationService NotificationService { get; }
        public ITranslationService TranslationService { get; }

        public FriendRequestService(IFriendRequestRepository friendRequestRepository,
            IUserRepository userRepository, INotificationService notificationService,
            ITranslationService translationService)
        {
            this.FriendRequestRepository = friendRequestRepository ?? throw new ArgumentNullException(nameof(friendRequestRepository));
            this.UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.NotificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            this.TranslationService = translationService ?? throw new ArgumentNullException(nameof(translationService));
        }

        public async Task AddFriendRequestAsync(string requestingUserId, string friendUserId)
        {
            var hasPendingRequest = await FriendRequestRepository.HasPendingFriendRequestAsync(friendUserId, requestingUserId);
            if(hasPendingRequest == false)
            {
                var requestingUser = await UserRepository.FindUserAsync(requestingUserId);
                var friendUser = await UserRepository.FindUserAsync(friendUserId);

                await FriendRequestRepository.AddFriendRequestAsync(friendUser.ToUserInfo(), requestingUser.ToUserInfo());

                // send push notification to inform user about friend request
                if(friendUser.PushInfo != null)
                {
                    var subject = await TranslationService.GetTranslationAsync(friendUser.Language, "FriendsRequest");
                    var message = await TranslationService.GetTranslationAsync(friendUser.Language, "SentFriendsRequest", requestingUser.Name);

                    NotificationService.SendMessage(new[] { friendUser.PushInfo }, new NotificationMessage(Constants.Urls.ApplicationIconUrl,
                        Constants.Urls.ApplicationIconUrl, Constants.Urls.FriendRequestApplicationUrl, subject, message));
                }
            }
        }

        public async Task AcceptFriendRequestAsync(string acceptingUserId, string requestingUserId)
        {
            await UserRepository.AddFriendAsync(acceptingUserId, requestingUserId);

            await FriendRequestRepository.DeleteFriendRequestAsync(acceptingUserId, requestingUserId);

            var acceptingUser = await UserRepository.FindUserAsync(acceptingUserId);
            var requestingUser = await UserRepository.FindUserAsync(requestingUserId);
            if (requestingUser.PushInfo != null)
            {
                var subject = await TranslationService.GetTranslationAsync(requestingUser.Language, "FriendsRequest");
                var message = await TranslationService.GetTranslationAsync(requestingUser.Language, "AcceptedFriendsRequest", requestingUser.Name);

                NotificationService.SendMessage(new[] { requestingUser.PushInfo }, new NotificationMessage(Constants.Urls.ApplicationIconUrl,
                    Constants.Urls.ApplicationIconUrl, Constants.Urls.FriendRequestApplicationUrl, subject, message));
            }
        }

        public Task DeclineFriendRequestAsync(string decliningUserId, string requestingUserId)
        {
            return FriendRequestRepository.DeleteFriendRequestAsync(decliningUserId, requestingUserId);
        }

   
    }
}
