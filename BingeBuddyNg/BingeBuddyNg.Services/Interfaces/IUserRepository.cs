using BingeBuddyNg.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User> GetUserAsync(string id);
        Task SaveUserAsync(User user);
        Task UpdateUserProfileAsync(User user);

        Task AddFriendAsync(string userId, string friendUserId);
        Task RemoveFriendAsync(string userId, string friendUserId);
    }
}
