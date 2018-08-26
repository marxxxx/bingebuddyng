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

        Task SaveStatisticsForUserAsync(UserStatistics userStatistics);
    }
}
