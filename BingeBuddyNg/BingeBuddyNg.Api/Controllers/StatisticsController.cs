using BingeBuddyNg.Api.Dto;
using BingeBuddyNg.Services.Statistics;
using BingeBuddyNg.Services.Statistics.Querys;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BingeBuddyNg.Api.Controllers
{
    [Route("api/User/{userId}/statistics")]
    public class StatisticsController : Controller
    {
        private readonly IMediator meditator;
        
        public StatisticsController(IMediator mediator)
        {
            meditator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("history")]
        public async Task<IEnumerable<UserStatisticHistoryDTO>> GetStatisticHistoryForUser(string userId)
        {
            var result = await meditator.Send(new GetStatisticHistoryForUserQuery(userId));
            return result;
        }

        [HttpGet("personalusageperweekday")]
        public async Task<IEnumerable<PersonalUsagePerWeekdayDTO>> GetPersonalUsagePerWeekday(string userId)
        {
            var result = await meditator.Send(new GetPersonalUsagePerWeekdayQuery(userId));
            return result;
        }
    }
}
