using System.Collections.Generic;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Statistics.DTO;
using BingeBuddyNg.Core.Statistics.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BingeBuddyNg.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class RankingController : Controller
    {
        private readonly ISender mediator;

        public RankingController(ISender mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("drinks")]
        public async Task<List<UserRankingDTO>> GetDrinkRanking()
        {
            var result = await this.mediator.Send(new GetDrinksRankingQuery());
            return result;
        }

        [HttpGet("score")]
        public async Task<List<UserRankingDTO>> GetScoreRanking()
        {
            var result = await this.mediator.Send(new GetScoreRankingQuery());
            return result;
        }

        [HttpGet("venue")]
        public async Task<IEnumerable<VenueRankingDTO>> GetVenueRanking()
        {
            var result = await this.mediator.Send(new GetVenueRankingQuery());
            return result;
        }
    }
}