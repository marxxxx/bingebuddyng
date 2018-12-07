using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Services.DTO;
using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        public async Task<List<UserRanking>> GetRanking()
        {
            var result = await this.RankingService.GetUserRankingAsync();
            return result;
        }
    }
}
