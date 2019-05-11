using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Statistics;
using BingeBuddyNg.Services.User;

namespace BingeBuddyNg.Services.Ranking
{
    public class UserRankingService : IUserRankingService
    {
        public IUserRepository UserRepository { get; }
        public IUserStatsRepository UserStatsRepository { get; }

        public UserRankingService(IUserRepository userRepository, 
            IUserStatsRepository userStatsRepository)
        {
            UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            UserStatsRepository = userStatsRepository ?? throw new ArgumentNullException(nameof(userStatsRepository));
        }

        public async Task<List<UserRanking>> GetDrinksRankingAsync()
        {
            var userStats = await this.UserStatsRepository.GetRankingStatisticsAsync();
            var userIds = userStats.Select(u => u.UserId).Distinct();
            var users = await this.UserRepository.GetUsersAsync(userIds);

            var result = userStats.Select(s => new UserRanking(users.First(u => u.Id == s.UserId).ToUserInfo(), s)).ToList();
            return result;
        }

        public async Task<List<UserRanking>> GetScoreRankingAsync()
        {
            var userStats = await this.UserStatsRepository.GetScoreStatisticsAsync();
            var userIds = userStats.Select(u => u.UserId).Distinct();
            var users = await this.UserRepository.GetUsersAsync(userIds);

            var result = userStats.Select(s => new UserRanking(users.First(u => u.Id == s.UserId).ToUserInfo(), s))
                .OrderByDescending( r => r.Statistics.Score)
                .ToList();
            return result;
        }
    }
}
