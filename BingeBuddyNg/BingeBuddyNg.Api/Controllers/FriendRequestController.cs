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
        private readonly IIdentityService identityService;
        private readonly IMediator mediator;

        public FriendRequestController(IIdentityService identityService, IMediator mediator)
        {
            this.identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("pending")]
        public async Task<List<FriendRequestDTO>> GetPendingFriendRequests()
        {
            var userId = identityService.GetCurrentUserId();

            var result = await mediator.Send(new GetPendingFriendsRequestsQuery(userId));
            return result;
        }

        [HttpGet("hasPending/{friendUserId}")]
        public async Task<bool> HasPendingFriendRequest(string friendUserId)
        {
            var userId = identityService.GetCurrentUserId();
            var result = await mediator.Send(new HasPendingFriendsRequestQuery(friendUserId, userId));
            return result;
        }

        [HttpPost("request/{friendUserId}")]
        public async Task AddFriendRequest(string friendUserId)
        {
            var userId = identityService.GetCurrentUserId();
            await mediator.Send(new AddFriendRequestCommand(userId, friendUserId));
        }

        [HttpPut("accept/{requestingUserId}")]
        public async Task AcceptFriendRequest(string requestingUserId)
        {
            var userId = identityService.GetCurrentUserId();
            await mediator.Send(new AcceptFriendRequestCommand(userId, requestingUserId));
        }

        [HttpPut("decline/{requestingUserId}")]
        public async Task DeclineFriendRequest(string requestingUserId)
        {
            var userId = identityService.GetCurrentUserId();
            await mediator.Send(new DeclineFriendRequestCommand(userId, requestingUserId));
        }
    }
}
