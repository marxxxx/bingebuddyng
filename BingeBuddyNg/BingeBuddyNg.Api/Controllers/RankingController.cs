using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Ranking.DTO;
using BingeBuddyNg.Core.Ranking.Queries;
using BingeBuddyNg.Services.Ranking;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BingeBuddyNg.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class RankingController : Controller
    {
        private readonly IMediator mediator;
        private readonly GetScoreRankingQuery getScoreRankingQuery;
        private readonly GetDrinksRankingQuery getDrinksRankingQuery;

        public RankingController(IMediator mediator, GetScoreRankingQuery getScoreRankingQuery, GetDrinksRankingQuery getDrinksRankingQuery)
        {
            this.mediator = mediator;
            this.getScoreRankingQuery = getScoreRankingQuery;
            this.getDrinksRankingQuery = getDrinksRankingQuery;
        }

        [HttpGet("drinks")]
        public async Task<List<UserRankingDTO>> GetDrinkRanking()
        {
            var result = await this.getDrinksRankingQuery.ExecuteAsync();
            return result;
        }

        [HttpGet("score")]
        public async Task<List<UserRankingDTO>> GetScoreRanking()
        {
            var result = await this.getScoreRankingQuery.ExecuteAsync();
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
