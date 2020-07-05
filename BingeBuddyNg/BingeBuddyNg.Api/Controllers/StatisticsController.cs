using System.Collections.Generic;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Statistics.DTO;
using BingeBuddyNg.Core.Statistics.Queries;
using BingeBuddyNg.Core.Statistics.Querys;
using Microsoft.AspNetCore.Mvc;

namespace BingeBuddyNg.Api.Controllers
{
    [Route("api/User/{userId}/statistics")]
    public class StatisticsController : Controller
    {
        private readonly GetStatisticHistoryForUserQuery getStatisticHistoryForUserQuery;
        private readonly GetPersonalUsagePerWeekdayQuery getPersonalUsagePerWeekdayQuery;
        
        public StatisticsController(GetStatisticHistoryForUserQuery getStatisticHistoryForUserQuery, GetPersonalUsagePerWeekdayQuery getPersonalUsagePerWeekdayQuery)
        {
            this.getStatisticHistoryForUserQuery = getStatisticHistoryForUserQuery;
            this.getPersonalUsagePerWeekdayQuery = getPersonalUsagePerWeekdayQuery;
        }

        [HttpGet("history")]
        public async Task<IEnumerable<UserStatisticHistoryDTO>> GetStatisticHistoryForUser(string userId)
        {
            var result = await getStatisticHistoryForUserQuery.ExecuteAsync(userId);
            return result;
        }

        [HttpGet("personalusageperweekday")]
        public async Task<IEnumerable<PersonalUsagePerWeekdayDTO>> GetPersonalUsagePerWeekday(string userId)
        {
            var result = await getPersonalUsagePerWeekdayQuery.ExecuteAsync(userId);
            return result;
        }
    }
}
