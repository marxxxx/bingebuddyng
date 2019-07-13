using BingeBuddyNg.Services.FriendsRequest;
using BingeBuddyNg.Services.FriendsRequest.Commands;
using BingeBuddyNg.Services.FriendsRequest.Querys;
using BingeBuddyNg.Services.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BingeBuddyNg.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class FriendRequestController : Controller
    {
        public IIdentityService IdentityService { get; set; }
        public IMediator Mediator { get; }

        public FriendRequestController(IIdentityService identityService, IMediator mediator)
        {
            this.IdentityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            this.Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        public async Task<List<FriendRequestDTO>> GetPendingFriendRequests()
        {
            var userId = IdentityService.GetCurrentUserId();

            var result = await Mediator.Send(new GetPendingFriendsRequestsQuery(userId));
            return result;
        }

        [HttpGet("[action]/{friendUserId}")]
        public async Task<bool> HasPendingFriendRequest(string friendUserId)
        {
            var userId = IdentityService.GetCurrentUserId();
            var result = await Mediator.Send(new HasPendingFriendsRequestQuery(friendUserId, userId));
            return result;
        }

        [HttpPost("request/{friendUserId}")]
        public async Task AddFriendRequest(string friendUserId)
        {
            var userId = IdentityService.GetCurrentUserId();
            await Mediator.Send(new AddFriendRequestCommand(userId, friendUserId));
        }

        [HttpPut("accept/{requestingUserId}")]
        public async Task AcceptFriendRequest(string requestingUserId)
        {
            var userId = IdentityService.GetCurrentUserId();
            await Mediator.Send(new AcceptFriendRequestCommand(userId, requestingUserId));
        }

        [HttpPut("decline/{requestingUserId}")]
        public async Task DeclineFriendRequest(string requestingUserId)
        {
            var userId = IdentityService.GetCurrentUserId();
            await Mediator.Send(new DeclineFriendRequestCommand(userId, requestingUserId));
        }
    }
}
