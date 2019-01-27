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
        public IRankingService RankingService { get; }
        public IIdentityService IdentityService { get; set; }

        public RankingController(IIdentityService identityService, IRankingService rankingService)
        {
            this.IdentityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            this.RankingService = rankingService ?? throw new ArgumentNullException(nameof(rankingService));
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
    }
}
