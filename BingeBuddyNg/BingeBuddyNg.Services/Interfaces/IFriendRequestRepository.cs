using BingeBuddyNg.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Interfaces
{
    public interface IFriendRequestRepository
    {
        Task<List<UserInfo>> GetFriendRequestsAsync(string userId);
        Task AddFriendRequestAsync(string userId, UserInfo requestingUser);
        Task DeleteFriendRequestAsync(string userId, string requestingUserId);
        Task<bool> HasPendingFriendRequestAsync(string userId, string requestingUserId);
    }
}
