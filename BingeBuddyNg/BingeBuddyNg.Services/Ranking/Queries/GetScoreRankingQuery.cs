using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Ranking.DTO;
using BingeBuddyNg.Core.Statistics;
using BingeBuddyNg.Core.User;
using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Services.User.Queries;
using Microsoft.WindowsAzure.Storage.Table;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Core.Ranking.Queries
{
    public class GetScoreRankingQuery
    {
        private readonly SearchUsersQuery getAllUsersQuery;
        private readonly IStorageAccessService storageAccessService;

        public GetScoreRankingQuery(SearchUsersQuery getAllUsersQuery, IStorageAccessService storageAccessService)
        {
            this.getAllUsersQuery = getAllUsersQuery;
            this.storageAccessService = storageAccessService;
        }

        public async Task<List<UserRankingDTO>> ExecuteAsync()
        {
            var userStats = await this.GetScoreStatisticsAsync();
            var userIds = userStats.Select(u => u.UserId).Distinct();
            var users = await this.getAllUsersQuery.ExecuteAsync(userIds);

            var result = userStats.Select(s => new UserRankingDTO(users.First(u => u.Id == s.UserId).ToUserInfoDTO(), s.ToDto()))
                .OrderByDescending(r => r.Statistics.Score)
                .ToList();

            return result;
        }

        private async Task<IEnumerable<UserStatistics>> GetScoreStatisticsAsync()
        {
            string whereClause = TableQuery.GenerateFilterConditionForInt(nameof(UserStatsTableEntity.Score), QueryComparisons.GreaterThan, 0);

            var queryResult = await this.storageAccessService.QueryTableAsync<UserStatsTableEntity>(TableNames.UserStats, whereClause);

            var result = queryResult.Where(s => s.Score.GetValueOrDefault() > 0)
                .OrderByDescending(r => r.Score)
                .Select(r => new UserStatistics(r.RowKey, r.CurrentAlcoholization, r.CurrentNightDrinks, r.Score, r.TotalDrinksLastMonth))
                .ToList();

            return result;
        }
    }

}
