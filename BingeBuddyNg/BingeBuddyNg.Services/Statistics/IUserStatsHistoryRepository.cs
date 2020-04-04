﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Statistics
{
    public interface IUserStatsHistoryRepository
    {
        Task<List<UserStatisticHistory>> GetStatisticHistoryForUserAsync(string userId);

        Task SaveStatisticsHistoryAsync(UserStatisticHistory userStatistics);
    }
}