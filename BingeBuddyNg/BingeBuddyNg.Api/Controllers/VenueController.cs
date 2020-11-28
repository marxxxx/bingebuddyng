using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Core.Venue.Commands;
using BingeBuddyNg.Core.Venue.DTO;
using BingeBuddyNg.Core.Venue.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BingeBuddyNg.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class VenueController : Controller
    {
        private readonly ISender mediator;
        private readonly IIdentityService identityService;

        public VenueController(IIdentityService identityService, ISender mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        }

        [HttpGet]
        public async Task<IEnumerable<VenueDTO>> SearchVenues(float latitude, float longitude)
        {
            var venues = await this.mediator.Send(new SearchVenuesQuery(latitude, longitude));
            return venues;
        }

        [HttpPost("update-current")]
        public async Task UpdateCurrentVenue([FromBody] VenueDTO venue)
        {
            var userId = this.identityService.GetCurrentUserId();
            await this.mediator.Send(new EnterVenueCommand(userId, venue));
        }

        [HttpPost("reset-current")]
        public async Task ResetCurrentVenue()
        {
            var userId = this.identityService.GetCurrentUserId();

            await this.mediator.Send(new LeaveVenueCommand(userId));
        }
    }
}