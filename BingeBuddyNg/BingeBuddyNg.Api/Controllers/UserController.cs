using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Services.DTO;
using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Models;
using BingeBuddyNg.Services.Models.Venue;
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

        [HttpGet]
        public async Task<List<UserInfo>> GetAllUsers(string filterText=null)
        {
            var users = await this.UserRepository.GetUsersAsync();
                        
            var userInfo = users.Select(u => u.ToUserInfo()).ToList();

            // TODO: Should soon be improved!
            if (!string.IsNullOrEmpty(filterText))
            {
                userInfo = userInfo.Where(u => u.UserName.Contains(filterText)).ToList();
            }

            return userInfo;
        }

        [HttpGet("{userId}")]
        public async Task<User> GetUser(string userId)
        {
            var user = await this.UserRepository.FindUserAsync(userId);
            return user;
        }
                
        [HttpPost]
        public async Task<UpdateUserResponseDTO> UpdateUserProfile([FromBody]User user)
        {
            await this.UserRepository.CreateOrUpdateUserAsync(user);

            var response = new UpdateUserResponseDTO(!user.Weight.HasValue, user.Gender == Gender.Unknown);
            return response;
        }
        

        [HttpDelete("{friendUserId}")]
        public Task RemoveFriend(string friendUserId)
        {
            var userId = IdentityService.GetCurrentUserId();
            return UserRepository.RemoveFriendAsync(userId, friendUserId);
        }

        [HttpPut("[action]")]
        public async Task SetFriendMuteState(string friendUserId, bool muteState)
        {
            var userId = IdentityService.GetCurrentUserId();
            var user = await UserRepository.FindUserAsync(userId);
            user.SetFriendMuteState(friendUserId, muteState);

            var mutedUser = await UserRepository.FindUserAsync(friendUserId);
            mutedUser.SetMutedByFriendState(userId, muteState);

            Task.WaitAll(UserRepository.UpdateUserAsync(user), UserRepository.UpdateUserAsync(mutedUser));
        }

    }
}
