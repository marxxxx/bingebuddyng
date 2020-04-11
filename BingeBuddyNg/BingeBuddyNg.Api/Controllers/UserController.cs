using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.User;
using BingeBuddyNg.Services.User.Commands;
using BingeBuddyNg.Services.User.Querys;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BingeBuddyNg.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IIdentityService identityService;
        private readonly IMediator mediator;

        public UserController(IIdentityService identityService,
            IMediator mediator)
        {
            this.identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        public async Task<List<UserInfoDTO>> GetAllUsers(string filterText = null)
        {
            var result = await mediator.Send(new GetAllUsersQuery(filterText));

            return result;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserDTO>> GetUser(string userId)
        {
            var user = await mediator.Send(new GetUserQuery(userId));
            if (user == null)
            {
                return NotFound($"User {userId} not found.");
            }
            return user;
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrUpdateUser([FromBody]CreateOrUpdateUserDTO user)
        {
            var userId = this.identityService.GetCurrentUserId();
            if (user.UserId != userId)
            {
                return Unauthorized();
            }
            await this.mediator.Send(new CreateOrUpdateUserCommand(user.UserId, user.Name, user.ProfileImageUrl, user.PushInfo, user.Language));
            return Ok();
        }

        [HttpPost("profile-image")]
        public async Task UpdateUserProfileImage(IFormFile file)
        {
            var currentUserId = this.identityService.GetCurrentUserId();

            await this.mediator.Send(new UpdateUserProfileImageCommand(currentUserId, file));
        }

        [HttpDelete("{friendUserId}/removefriend")]
        public async Task RemoveFriend(string friendUserId)
        {
            var userId = identityService.GetCurrentUserId();
            await mediator.Send(new RemoveFriendCommand(userId, friendUserId));
        }

        [HttpPut("{friendUserId}/mute")]
        public async Task SetFriendMuteState(string friendUserId, bool state)
        {
            var userId = identityService.GetCurrentUserId();
            await mediator.Send(new SetFriendMuteStateCommand(userId, friendUserId, state));
        }

        [HttpDelete("myself")]
        public async Task DeleteMyself()
        {
            var userId = identityService.GetCurrentUserId();
            await mediator.Send(new DeleteUserCommand(userId));
        }
    }
}
