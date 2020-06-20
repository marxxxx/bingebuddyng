using System.Collections.Generic;
using System.Threading.Tasks;
using BingeBuddyNg.Core.User;
using BingeBuddyNg.Services.FriendsRequest;

namespace BingeBuddyNg.Core.FriendsRequest
{
    public interface IFriendRequestRepository
    {
        Task<List<FriendRequestDTO>> GetFriendRequestsAsync(string userId);

        Task AddFriendRequestAsync(UserInfo friend, UserInfo requestingUser);

        Task DeleteFriendRequestAsync(string userId, string requestingUserId);

        Task<bool> HasPendingFriendRequestAsync(string userId, string requestingUserId);
    }
}
