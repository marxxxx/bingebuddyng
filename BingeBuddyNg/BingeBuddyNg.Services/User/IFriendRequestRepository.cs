using System.Collections.Generic;
using System.Threading.Tasks;
using BingeBuddyNg.Core.FriendsRequest.Persistence;
using BingeBuddyNg.Core.User.Persistence;

namespace BingeBuddyNg.Core.User
{
    public interface IFriendRequestRepository
    {
        Task<List<FriendRequestEntity>> GetFriendRequestsAsync(string userId);

        Task AddFriendRequestAsync(UserInfo friend, UserInfo requestingUser);

        Task DeleteFriendRequestAsync(string userId, string requestingUserId);

        Task<bool> HasPendingFriendRequestAsync(string userId, string requestingUserId);
    }
}
