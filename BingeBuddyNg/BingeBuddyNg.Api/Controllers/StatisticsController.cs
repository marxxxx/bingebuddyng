using System.Collections.Generic;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Statistics.DTO;
using BingeBuddyNg.Core.Statistics.Queries;
using BingeBuddyNg.Core.Statistics.Querys;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BingeBuddyNg.Api.Controllers
{
    [Route("api/User/{userId}/statistics")]
    public class StatisticsController : Controller
    {
        private readonly ISender mediator;

        public StatisticsController(ISender mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("history")]
        public async Task<IEnumerable<UserStatisticHistoryDTO>> GetStatisticHistoryForUser(string userId)
        {
            return await this.mediator.Send(new GetStatisticHistoryForUserQuery(userId));
        }

        [HttpGet("personalusageperweekday")]
        public async Task<IEnumerable<PersonalUsagePerWeekdayDTO>> GetPersonalUsagePerWeekday(string userId)
        {
            var result = await mediator.Send(new GetPersonalUsagePerWeekdayQuery(userId));
            return result;
        }
    }
}