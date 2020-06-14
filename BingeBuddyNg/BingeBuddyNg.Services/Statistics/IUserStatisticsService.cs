using System.Threading.Tasks;
using BingeBuddyNg.Services.User;

namespace BingeBuddyNg.Services.Statistics
{
    public interface IUserStatisticsService
    {
        Task UpdateRankingForUserAsync(string userId);

        Task<UserStatistics> UpdateStatsForUserAsync(string userId, Gender gender, int? weight);
    }
}
