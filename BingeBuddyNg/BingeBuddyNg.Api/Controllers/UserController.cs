using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BingeBuddyNg.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        public IUserRepository UserRepository { get; }
        public IUserService UserService { get; }
        public IActivityRepository ActivityRepository { get; }
        public IIdentityService IdentityService { get; set; }

        public UserController(IIdentityService identityService, IUserRepository userRepository, 
            IUserService userService,
            IActivityRepository activityRepository)
        {
            this.IdentityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            this.UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.UserService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.ActivityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
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
        public async Task<ActionResult<User>> GetUser(string userId)
        {
            var user = await this.UserRepository.FindUserAsync(userId);
            if(user == null)
            {
                return NotFound($"User {userId} not found.");
            }
            return user;
        }
                
        [HttpPost]
        public async Task<ActionResult<UpdateUserResponseDTO>> UpdateUserProfile([FromBody]User user)
        {
            var userId = this.IdentityService.GetCurrentUserId();
            if(user.Id != userId)
            {
                return Unauthorized();
            }
            var result = await this.UserService.UpdateUserProfileAsync(user);
            return result;
        }


        [HttpPost("[action]")]
        public async Task UpdateUserProfilePic(IFormFile file)
        {
            var currentUserId = this.IdentityService.GetCurrentUserId();
           
            await this.UserService.UpdateUserProfilePicAsync(currentUserId, file);
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
