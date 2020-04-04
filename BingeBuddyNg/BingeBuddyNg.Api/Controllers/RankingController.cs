﻿using BingeBuddyNg.Services.Infrastructure;
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
        private readonly IIdentityService identityService;
        private readonly IMediator mediator;

        public RankingController(IIdentityService identityService, IMediator mediator)
        {
            this.identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
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
