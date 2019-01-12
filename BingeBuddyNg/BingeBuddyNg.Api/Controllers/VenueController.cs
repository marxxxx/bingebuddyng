using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Models.Venue;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BingeBuddyNg.Api.Controllers
{
    [Route("api/[controller]")]
    public class VenueController : Controller
    {
        public IFourSquareService FourSquareService { get; }

        public VenueController(IFourSquareService fourSquareService)
        {
            this.FourSquareService = fourSquareService ?? throw new ArgumentNullException(nameof(fourSquareService));
        }
                

        [HttpGet]
        public async Task<IEnumerable<VenueModel>> Get(float latitude, float longitude)
        {
            var venues = await this.FourSquareService.SearchVenuesAsync(latitude, longitude);
            return venues;
        }
    }
}
