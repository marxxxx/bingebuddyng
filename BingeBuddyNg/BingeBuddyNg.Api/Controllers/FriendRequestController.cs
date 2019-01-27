using BingeBuddyNg.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Services.FriendsRequest;
using BingeBuddyNg.Services.Infrastructure;

namespace BingeBuddyNg.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class FriendRequestController : Controller
    {
        public IFriendRequestService FriendRequestService { get; set; }
        public IIdentityService IdentityService { get; set; }
        public IFriendRequestRepository FriendRequestRepository { get; set; }

        public FriendRequestController(IIdentityService identityService, IFriendRequestService friendRequestService,
            IFriendRequestRepository friendRequestRepository)
        {
            this.IdentityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            this.FriendRequestRepository = friendRequestRepository ?? throw new ArgumentNullException(nameof(friendRequestRepository));
            this.FriendRequestService = friendRequestService ?? throw new ArgumentNullException(nameof(friendRequestService));
        }

        [HttpGet]
        public Task<List<FriendRequestInfo>> GetPendingFriendRequests()
        {
            var userId = IdentityService.GetCurrentUserId();

            return FriendRequestRepository.GetFriendRequestsAsync(userId);
        }

        [HttpGet("[action]/{friendUserId}")]
        public Task<bool> HasPendingFriendRequest(string friendUserId)
        {
            var userId = IdentityService.GetCurrentUserId();
            return FriendRequestRepository.HasPendingFriendRequestAsync(friendUserId, userId);
        }

        [HttpPost("request/{friendUserId}")]
        public Task AddFriendRequest(string friendUserId)
        {
            var userId = IdentityService.GetCurrentUserId();
            return FriendRequestService.AddFriendRequestAsync(userId, friendUserId);
        }


        [HttpPut("accept/{requestingUserId}")]
        public Task AcceptFriendRequest(string requestingUserId)
        {
            var userId = IdentityService.GetCurrentUserId();
            return FriendRequestService.AcceptFriendRequestAsync(userId, requestingUserId);
        }

        [HttpPut("decline/{requestingUserId}")]
        public Task DeclineFriendRequest(string requestingUserId)
        {
            var userId = IdentityService.GetCurrentUserId();
            return FriendRequestService.DeclineFriendRequestAsync(userId, requestingUserId);
        }
    }
}
