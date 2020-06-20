using System.Threading.Tasks;
using BingeBuddyNg.Core.User;

namespace BingeBuddyNg.Core.Statistics
{
    public interface IUserStatisticsService
    {
        Task UpdateRankingForUserAsync(string userId);

        Task<UserStatistics> UpdateStatsForUserAsync(string userId, Gender gender, int? weight);
    }
}
