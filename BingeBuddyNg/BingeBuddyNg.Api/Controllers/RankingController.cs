using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.Ranking;
using BingeBuddyNg.Services.Ranking.Querys;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace BingeBuddyNg.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class RankingController : Controller
    {
        public IIdentityService IdentityService { get; set; }
        public IMediator Mediator { get; }

        public RankingController(IIdentityService identityService, IMediator mediator)
        {
            this.IdentityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            this.Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("[action]")]
        public async Task<List<UserRankingDTO>> GetDrinkRanking()
        {
            var result = await this.Mediator.Send(new GetDrinksRankingQuery());
            return result;
        }

        [HttpGet("[action]")]
        public async Task<List<UserRankingDTO>> GetScoreRanking()
        {
            var result = await this.Mediator.Send(new GetScoreRankingQuery());
            return result;
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<VenueRankingDTO>> GetVenueRanking()
        {
            var result = await this.Mediator.Send(new GetVenueRankingQuery());
            return result;
        }
    }
}
