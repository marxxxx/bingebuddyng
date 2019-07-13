using System.Collections.Generic;
using System.Threading.Tasks;
using BingeBuddyNg.Services.User;

namespace BingeBuddyNg.Services.FriendsRequest
{
    public interface IFriendRequestRepository
    {
        Task<List<FriendRequestDTO>> GetFriendRequestsAsync(string userId);
        Task AddFriendRequestAsync(UserInfo friend, UserInfo requestingUser);
        Task DeleteFriendRequestAsync(string userId, string requestingUserId);
        Task<bool> HasPendingFriendRequestAsync(string userId, string requestingUserId);
    }
}
