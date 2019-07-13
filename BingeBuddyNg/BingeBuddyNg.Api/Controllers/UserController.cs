using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.User;
using BingeBuddyNg.Services.User.Commands;
using BingeBuddyNg.Services.User.Querys;
using MediatR;
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
        public IIdentityService IdentityService { get; }
        public IMediator Mediator { get; }

        public UserController(IIdentityService identityService, 
            IMediator mediator)
        {
            this.IdentityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            this.Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        public async Task<List<UserInfoDTO>> GetAllUsers(string filterText=null)
        {
            var result = await Mediator.Send(new GetAllUsersQuery(filterText));

            return result;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserDTO>> GetUser(string userId)
        {
            var user = await Mediator.Send(new GetUserQuery(userId));
            if(user == null)
            {
                return NotFound($"User {userId} not found.");
            }
            return user;
        }

        [HttpPost]
        public async Task<ActionResult> UpdateUserProfile([FromBody]CreateOrUpdateUserDTO user)
        {
            var userId = this.IdentityService.GetCurrentUserId();
            if(user.UserId != userId)
            {
                return Unauthorized();
            }
            await this.Mediator.Send(new CreateOrUpdateUserCommand(user.UserId, user.Name, user.ProfileImageUrl, user.PushInfo, user.Language));
            return Ok();
        }


        [HttpPost("[action]")]
        public async Task UpdateUserProfilePic(IFormFile file)
        {
            var currentUserId = this.IdentityService.GetCurrentUserId();
           
            await this.Mediator.Send(new UpdateUserProfileImageCommand(currentUserId, file));
        }


        [HttpDelete("{friendUserId}")]
        public async Task RemoveFriend(string friendUserId)
        {
            var userId = IdentityService.GetCurrentUserId();
            await Mediator.Send(new RemoveFriendCommand(userId, friendUserId));
        }

        [HttpPut("[action]")]
        public async Task SetFriendMuteState(string friendUserId, bool muteState)
        {
            var userId = IdentityService.GetCurrentUserId();
            await Mediator.Send(new SetFriendMuteStateCommand(userId, friendUserId, muteState));            
        }

    }
}
