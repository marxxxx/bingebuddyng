using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.Venue;
using MediatR;
using BingeBuddyNg.Services.Venue.Querys;
using BingeBuddyNg.Services.Venue.Commands;


namespace BingeBuddyNg.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class VenueController : Controller
    {
        private readonly IMediator mediator;
        private readonly IIdentityService identityService;

        public VenueController(IIdentityService identityService, IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        }

        [HttpGet]
        public async Task<IEnumerable<VenueModel>> SearchVenues(float latitude, float longitude)
        {
            var venues = await this.mediator.Send(new SearchVenuesQuery(latitude, longitude));
            return venues;
        }

        [HttpPost("update-current")]
        public async Task UpdateCurrentVenue([FromBody]VenueModel venue)
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
