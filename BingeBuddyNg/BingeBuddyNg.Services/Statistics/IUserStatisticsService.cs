using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Statistics
{
    public interface IUserStatisticsService
    {
        Task UpdateRankingForUserAsync(string userId);

        Task<UserStatistics> UpdateStatsForUserAsync(User.User user);
    }
}
