﻿using BingeBuddyNg.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User> FindUserAsync(string id);
        Task UpdateUserAsync(User user);
        Task CreateOrUpdateUserAsync(User user);

        Task AddFriendAsync(string userId, string friendUserId);
        Task RemoveFriendAsync(string userId, string friendUserId);
    }
}
