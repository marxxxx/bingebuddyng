using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.User;
using BingeBuddyNg.Services.User.Commands;
using BingeBuddyNg.Services.User.Queries;
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
        private readonly IMediator mediator;
        private readonly ISearchUsersQuery getUsersQuery;
        private readonly IUserRepository userRepository;

        public UserController(IIdentityService identityService,
            IMediator mediator,
            ISearchUsersQuery getUsersQuery, 
            IUserRepository userRepository)
        {
            this.identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.getUsersQuery = getUsersQuery ?? throw new ArgumentNullException(nameof(getUsersQuery));
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        [HttpGet]
        public async Task<List<UserInfoDTO>> GetAllUsers(string filterText = null)
        {
            var result = await getUsersQuery.ExecuteAsync(filterText: filterText);

            return result.Select(r=>r.ToUserInfoDTO()).ToList();
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserDTO>> GetUser(string userId)
        {
            var user = await userRepository.GetUserAsync(userId);
            return user.ToDto();
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
