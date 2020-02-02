using BingeBuddyNg.Api.Dto;
using BingeBuddyNg.Services.Statistics;
using BingeBuddyNg.Services.Statistics.Querys;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BingeBuddyNg.Api.Controllers
{
    [Route("api/[controller]")]
    public class StatisticsController : Controller
    {
        private ILogger<StatisticsController> logger;

        public IMediator Mediator { get; }
        
        public StatisticsController(IMediator mediator, ILogger<StatisticsController> logger)
        {
            Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("{userId}")]
        public async Task<IEnumerable<UserStatisticHistoryDTO>> GetStatisticHistoryForUser(string userId)
        {
            var result = await Mediator.Send(new GetStatisticHistoryForUserQuery(userId));
            return result;
        }

        [HttpGet("personalusageperweekday/{userId}")]
        public async Task<IEnumerable<PersonalUsagePerWeekdayDTO>> GetPersonalUsagePerWeekday(string userId)
        {
            var result = await Mediator.Send(new GetPersonalUsagePerWeekdayQuery(userId));
            return result;
        }
    }
}
