using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Api.Dto;
using BingeBuddyNg.Services.Statistics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BingeBuddyNg.Api.Controllers
{
    [Route("api/[controller]")]
    public class StatisticsController : Controller
    {
        private ILogger<StatisticsController> logger;

        public IUserStatsHistoryRepository UserStatsHistoryRepository { get; }

        
        public StatisticsController(IUserStatsHistoryRepository userStatsHistoryRepository, ILogger<StatisticsController> logger)
        {
            UserStatsHistoryRepository = userStatsHistoryRepository ?? throw new ArgumentNullException(nameof(userStatsHistoryRepository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("{userId}")]
        public async Task<IEnumerable<UserStatisticHistoryDTO>> GetStatisticHistoryForUser(string userId)
        {
            var history = await this.UserStatsHistoryRepository.GetStatisticHistoryForUserAsync(userId);
            var result = history.Select(h => new UserStatisticHistoryDTO(h.Timestamp, h.CurrentAlcLevel)).ToList();
            return result;
        }
    }
}
