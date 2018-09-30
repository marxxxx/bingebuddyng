using BingeBuddyNg.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Interfaces
{
    public interface IFriendRequestRepository
    {
        Task<List<FriendRequestInfo>> GetFriendRequestsAsync(string userId);
        Task AddFriendRequestAsync(UserInfo friend, UserInfo requestingUser);
        Task DeleteFriendRequestAsync(string userId, string requestingUserId);
        Task<bool> HasPendingFriendRequestAsync(string userId, string requestingUserId);
    }
}
