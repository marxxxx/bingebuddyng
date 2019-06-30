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
        public IMediator Mediator { get; }
        public IIdentityService IdentityService { get; set; }        

        public VenueController(IIdentityService identityService, IMediator mediator)
        {
            Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            IdentityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        }

        [HttpGet]
        public async Task<IEnumerable<VenueModel>> SearchVenues(float latitude, float longitude)
        {
            var venues = await this.Mediator.Send(new SearchVenuesQuery(latitude, longitude));
            return venues;
        }

        [HttpPost("[action]")]
        public async Task UpdateCurrentVenue([FromBody]VenueModel venue)
        {
            var userId = this.IdentityService.GetCurrentUserId();
            await this.Mediator.Send(new EnterVenueCommand(userId, venue));
        }

        [HttpPost("[action]")]
        public async Task ResetCurrentVenue()
        {
            var userId = this.IdentityService.GetCurrentUserId();

            await this.Mediator.Send(new LeaveVenueCommand(userId));
        }
    }
}
