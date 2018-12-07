using BingeBuddyNg.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Interfaces
{
    public interface IUserStatsRepository
    {
        Task<List<UserStatistics>> GetStatisticsAsync(IEnumerable<string> userId);
        Task<UserStatistics> GetStatisticsAsync(string userId);
        Task<List<UserStatistics>> GetRankingStatisticsAsync();
        Task SaveStatisticsForUserAsync(UserStatistics userStatistics);
        Task UpdateTotalDrinkCountLastMonthAsync(string userId, int count);

    }
}
