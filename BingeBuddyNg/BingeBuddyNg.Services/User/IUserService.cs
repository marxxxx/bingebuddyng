using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.User
{
    public interface IUserService
    {
        Task<UpdateUserResponseDTO> UpdateUserProfileAsync(User user);
        Task UpdateUserProfilePicAsync(string userId, IFormFile file);
    }
}