using BingeBuddyNg.Services.Interfaces;
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
                var requestingUser = await UserRepository.GetUserAsync(requestingUserId);

                await FriendRequestRepository.AddFriendRequestAsync(friendUserId, requestingUser.ToUserInfo());

                // send push notification to inform user about friend request
                var friendUser = await UserRepository.GetUserAsync(friendUserId);
                if(friendUser.PushInfo != null)
                {
                    NotificationService.SendMessage(new[] { friendUser.PushInfo }, new Models.NotificationMessage(Constants.FriendRequestNotificationIconUrl,
                        Constants.FriendRequestNotificationIconUrl, Constants.FriendRequestApplicationUrl, "Freundschaftsanfrage",
                        $"{requestingUser.Name} hat dir eine Freundschaftsanfrage geschickt!"));
                }
            }
        }

        public async Task AcceptFriendRequestAsync(string acceptingUserId, string requestingUserId)
        {
            await FriendRequestRepository.DeleteFriendRequestAsync(acceptingUserId, requestingUserId);

            await UserRepository.AddFriendAsync(acceptingUserId, requestingUserId);

            var acceptingUser = await UserRepository.GetUserAsync(acceptingUserId);
            var requestingUser = await UserRepository.GetUserAsync(requestingUserId);
            if (requestingUser.PushInfo != null)
            {
                NotificationService.SendMessage(new[] { requestingUser.PushInfo }, new Models.NotificationMessage(Constants.FriendRequestNotificationIconUrl,
                    Constants.FriendRequestNotificationIconUrl, Constants.FriendRequestApplicationUrl, "Freundschaftsanfrage",
                    $"{acceptingUser.Name} hat deine Freundschaftsanfrage akzeptiert!"));
            }
        }

        public Task DeclineFriendRequestAsync(string decliningUserId, string requestingUserId)
        {
            return FriendRequestRepository.DeleteFriendRequestAsync(decliningUserId, requestingUserId);
        }

   
    }
}
