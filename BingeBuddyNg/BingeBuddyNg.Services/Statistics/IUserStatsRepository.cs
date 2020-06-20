﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace BingeBuddyNg.Core.Statistics
{
    public interface IUserStatsRepository
    {
        Task<List<UserStatistics>> GetStatisticsAsync(IEnumerable<string> userId);

        Task<List<UserStatistics>> GetScoreStatisticsAsync();

        Task<List<UserStatistics>> GetRankingStatisticsAsync();

        Task SaveStatisticsForUserAsync(UserStatistics userStatistics);

        Task UpdateTotalDrinkCountLastMonthAsync(string userId, int count);

        Task IncreaseScoreAsync(string userId, int additionalScore);
    }
}
