using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Models;
using BingeBuddyNg.Services.Models.Venue;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BingeBuddyNg.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class VenueController : Controller
    {
        public IVenueService VenueService { get; }
        public IIdentityService IdentityService { get; set; }
        

        public VenueController(IVenueService venueService, IIdentityService identityService)
        {
            VenueService = venueService ?? throw new ArgumentNullException(nameof(venueService));
            IdentityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        }


        [HttpGet]
        public async Task<IEnumerable<VenueModel>> SearchVenues(float latitude, float longitude)
        {
            var venues = await this.VenueService.SearchVenuesAsync(latitude, longitude);
            return venues;
        }

        [HttpPost("[action]")]
        public async Task UpdateCurrentVenue([FromBody]VenueModel venue)
        {
            var userId = this.IdentityService.GetCurrentUserId();
            await this.VenueService.UpdateVenueForUserAsync(userId, venue);
        }

        [HttpPost("[action]")]
        public async Task ResetCurrentVenue()
        {
            var userId = this.IdentityService.GetCurrentUserId();

            await this.VenueService.ResetVenueForUserAsync(userId);
            
        }
    }
}
