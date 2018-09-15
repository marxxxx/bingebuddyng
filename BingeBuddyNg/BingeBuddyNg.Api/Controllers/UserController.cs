using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Services.DTO;
using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BingeBuddyNg.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        public IUserRepository UserRepository { get; }
        public IIdentityService IdentityService { get; set; }

        public UserController(IIdentityService identityService, IUserRepository userRepository)
        {
            this.IdentityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            this.UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        [HttpGet("{userId}")]
        public async Task<User> GetUser(string userId)
        {
            var user = await this.UserRepository.GetUserAsync(userId);
            return user;
        }
                
        [HttpPost]
        public async Task<UpdateUserResponseDTO> UpdateUserProfile([FromBody]User user)
        {
            await this.UserRepository.UpdateUserProfileAsync(user);

            var response = new UpdateUserResponseDTO(!user.Weight.HasValue, user.Gender == Gender.Unknown);
            return response;
        }

        [HttpDelete("{friendUserId}")]
        public Task RemoveFriend(string friendUserId)
        {
            var userId = IdentityService.GetCurrentUserId();
            return UserRepository.RemoveFriendAsync(userId, friendUserId);
        }

    }
}
