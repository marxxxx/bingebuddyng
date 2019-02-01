using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.Ranking;
using BingeBuddyNg.Services.User;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BingeBuddyNg.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class RankingController : Controller
    {
        public IUserRankingService RankingService { get; }
        public IIdentityService IdentityService { get; set; }
        public IVenueRankingService VenueRankingService { get; set; }

        public RankingController(IIdentityService identityService, IUserRankingService rankingService, IVenueRankingService venueRankingService)
        {
            this.IdentityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            this.RankingService = rankingService ?? throw new ArgumentNullException(nameof(rankingService));
            this.VenueRankingService = venueRankingService ?? throw new ArgumentNullException(nameof(venueRankingService));
        }

        [HttpGet("[action]")]
        public async Task<List<UserRanking>> GetDrinkRanking()
        {
            var result = await this.RankingService.GetDrinksRankingAsync();
            return result;
        }

        [HttpGet("[action]")]
        public async Task<List<UserRanking>> GetScoreRanking()
        {
            var result = await this.RankingService.GetScoreRankingAsync();
            return result;
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<VenueRanking>> GetVenueRanking()
        {
            var result = await this.VenueRankingService.GetVenueRankingAsync();
            return result;
        }
    }
}
