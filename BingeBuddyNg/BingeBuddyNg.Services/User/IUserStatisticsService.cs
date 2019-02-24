using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.User
{
    public interface IUserStatisticsService
    {
        Task UpdateRankingForUserAsync(string userId);
        Task<UserStatistics> UpdateStatsForUserAsync(User user);
    }
}
