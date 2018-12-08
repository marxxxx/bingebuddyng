using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services
{
    public class FriendRequestService : IFriendRequestService
    {
        public IFriendRequestRepository FriendRequestRepository { get; }
        public IUserRepository UserRepository { get; }
        public INotificationService NotificationService { get; }

        public FriendRequestService(IFriendRequestRepository friendRequestRepository,
            IUserRepository userRepository, INotificationService notificationService)
        {
            this.FriendRequestRepository = friendRequestRepository ?? throw new ArgumentNullException(nameof(friendRequestRepository));
            this.UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.NotificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
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
                    NotificationService.SendMessage(new[] { friendUser.PushInfo }, new Models.NotificationMessage(Constants.Urls.ApplicationIconUrl,
                        Constants.Urls.ApplicationIconUrl, Constants.Urls.FriendRequestApplicationUrl, "Freundschaftsanfrage",
                        $"{requestingUser.Name} hat dir eine Freundschaftsanfrage geschickt!"));
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
                //TODO: Localize
                NotificationService.SendMessage(new[] { requestingUser.PushInfo }, new Models.NotificationMessage(Constants.Urls.ApplicationIconUrl,
                    Constants.Urls.ApplicationIconUrl, Constants.Urls.FriendRequestApplicationUrl, "Freundschaftsanfrage",
                    $"{acceptingUser.Name} hat deine Freundschaftsanfrage akzeptiert!"));
            }
        }

        public Task DeclineFriendRequestAsync(string decliningUserId, string requestingUserId)
        {
            return FriendRequestRepository.DeleteFriendRequestAsync(decliningUserId, requestingUserId);
        }

   
    }
}
