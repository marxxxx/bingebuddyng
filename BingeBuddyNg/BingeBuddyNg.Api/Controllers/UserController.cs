using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity.DTO;
using BingeBuddyNg.Core.FriendsRequest.Commands;
using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Core.User.Commands;
using BingeBuddyNg.Core.User.DTO;
using BingeBuddyNg.Core.User.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BingeBuddyNg.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IIdentityService identityService;
        private readonly ISender mediator;

        public UserController(IIdentityService identityService, ISender mediator)
        {
            this.identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        public async Task<List<UserInfoDTO>> GetAllUsers(string filterText = null)
        {
            return await this.mediator.Send(new GetAllUsersQuery(filterText));
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserDTO>> GetUser(string userId)
        {
            return await this.mediator.Send(new GetUserQuery(userId));
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrUpdateUser([FromBody] CreateOrUpdateUserDTO user)
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
            var userId = this.identityService.GetCurrentUserId();
            await this.mediator.Send(new UpdateUserProfileImageCommand(userId, file));
        }

        [HttpDelete("{friendUserId}/removefriend")]
        public async Task RemoveFriend(string friendUserId)
        {
            var userId = identityService.GetCurrentUserId();
            await mediator.Send(new RemoveFriendCommand(userId, friendUserId));
        }

        [HttpPost("{friendUserId}/request")]
        public async Task AddFriendRequest(string friendUserId)
        {
            var userId = identityService.GetCurrentUserId();
            await mediator.Send(new AddFriendRequestCommand(userId, friendUserId));
        }

        [HttpPut("{friendUserId}/accept")]
        public async Task AcceptFriendRequest(string friendUserId)
        {
            var userId = identityService.GetCurrentUserId();
            await mediator.Send(new AcceptFriendRequestCommand(userId, friendUserId));
        }

        [HttpPut("{friendUserId}/decline")]
        public async Task DeclineFriendRequest(string friendUserId)
        {
            var userId = identityService.GetCurrentUserId();
            await mediator.Send(new DeclineFriendRequestCommand(userId, friendUserId));
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