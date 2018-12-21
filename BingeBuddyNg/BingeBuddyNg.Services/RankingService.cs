using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services
{
    public class RankingService : IRankingService
    {
        public IUserRepository UserRepository { get; }
        public IUserStatsRepository UserStatsRepository { get; }

        public RankingService(IUserRepository userRepository, IUserStatsRepository userStatsRepository)
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

            var result = userStats.Select(s => new UserRanking(users.First(u => u.Id == s.UserId).ToUserInfo(), s)).ToList();
            return result;
        }
    }
}
