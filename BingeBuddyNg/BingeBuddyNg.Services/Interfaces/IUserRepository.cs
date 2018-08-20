using BingeBuddyNg.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserAsync(string id);
        Task SaveUserAsync(User user);

    }
}
