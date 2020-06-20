using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Ranking.DTO;
using BingeBuddyNg.Core.Statistics;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.User;
using BingeBuddyNg.Services.User.Queries;
using Microsoft.WindowsAzure.Storage.Table;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Core.Ranking.Queries
{
    public class GetDrinksRankingQuery
    {
        private readonly SearchUsersQuery getUsersQuery;
        private readonly IStorageAccessService storageAccessService;

        public GetDrinksRankingQuery(SearchUsersQuery getUsersQuery, IStorageAccessService storageAccessService)
        {
            this.getUsersQuery = getUsersQuery;
            this.storageAccessService = storageAccessService;
        }

        public async Task<List<UserRankingDTO>> ExecuteAsync()
        {
            string whereClause = TableQuery.GenerateFilterConditionForInt(nameof(UserStatsTableEntity.TotalDrinksLastMonth), QueryComparisons.GreaterThan, 0);
            var queryResult = await storageAccessService.QueryTableAsync<UserStatsTableEntity>(TableNames.UserStats, whereClause);

            var userStats = queryResult.OrderByDescending(r => r.TotalDrinksLastMonth)
                .Select(r => new UserStatistics(r.RowKey, r.CurrentAlcoholization, r.CurrentNightDrinks, r.Score, r.TotalDrinksLastMonth))
                .ToList();

            var userIds = userStats.Select(u => u.UserId).Distinct();
            var users = await this.getUsersQuery.ExecuteAsync(userIds);

            var result = userStats.Select(s => new UserRankingDTO(users.Select(u => new UserInfoDTO(u.Id, u.Name)).First(u => u.UserId == s.UserId), s.ToDto())).ToList();
            return result;
        }
    }
}
