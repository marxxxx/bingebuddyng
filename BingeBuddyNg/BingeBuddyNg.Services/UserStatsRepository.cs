using BingeBuddyNg.Services.Entitys;
using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Models;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services
{
    public class UserStatsRepository : IUserStatsRepository
    {
        private const string TableName = "userstats";
        private const string PartitionKeyValue = "UserStats";

        public StorageAccessService StorageAccessService { get; }

        public UserStatsRepository(StorageAccessService storageAccessService)
        {
            StorageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
        }


        public async Task<List<UserStatistics>> GetStatisticsAsync(IEnumerable<string> userId)
        {
            var tasks = userId.Select(u => GetStatisticsAsync(u));
            var result = await Task.WhenAll(tasks);
            return result.ToList();
        }

        public async Task<UserStatistics> GetStatisticsAsync(string userId)
        {
            var result = await StorageAccessService.GetTableEntityAsync<UserStatsTableEntity>(TableName, PartitionKeyValue, userId);
            if (result != null)
            {
                return new UserStatistics(userId, result.CurrentAlcoholization, result.CurrentNightDrinks, result.Score, result.TotalDrinksLastMonth);
            }
            else
            {
                return new UserStatistics(userId);
            }

        }

        public async Task<List<UserStatistics>> GetRankingStatisticsAsync()
        {
            string whereClause = TableQuery.GenerateFilterConditionForInt(nameof(UserStatsTableEntity.TotalDrinksLastMonth), QueryComparisons.GreaterThan, 0);
            var queryResult = await StorageAccessService.QueryTableAsync<UserStatsTableEntity>(TableName, whereClause);

            var result = queryResult.OrderByDescending(r => r.TotalDrinksLastMonth)
                .Select(r => new UserStatistics(r.RowKey, r.CurrentAlcoholization, r.CurrentNightDrinks, r.Score, r.TotalDrinksLastMonth))
                .ToList();

            return result;
        }

        public async Task<List<UserStatistics>> GetScoreStatisticsAsync()
        {
            string whereClause = TableQuery.GenerateFilterConditionForInt(nameof(UserStatsTableEntity.Score), QueryComparisons.GreaterThan, 0);

            var queryResult = await StorageAccessService.QueryTableAsync<UserStatsTableEntity>(TableName, whereClause);

            var result = queryResult.Where(s=>s.Score.GetValueOrDefault() > 0)
                .OrderByDescending(r => r.Score)
                .Select(r => new UserStatistics(r.RowKey, r.CurrentAlcoholization, r.CurrentNightDrinks, r.Score, r.TotalDrinksLastMonth))
                .ToList();

            return result;
        }

        public Task SaveStatisticsForUserAsync(UserStatistics userStatistics)
        {
            var table = StorageAccessService.GetTableReference(TableName);

            var entity = new UserStatsTableEntity(PartitionKeyValue, userStatistics.UserId,
                userStatistics.CurrentAlcoholization, userStatistics.CurrentNightDrinks,
                userStatistics.Score,
                userStatistics.TotalDrinksLastMonth);

            TableOperation saveOperation = TableOperation.InsertOrMerge(entity);

            return table.ExecuteAsync(saveOperation);
        }

        public async Task UpdateTotalDrinkCountLastMonthAsync(string userId, int count)
        {
            var table = StorageAccessService.GetTableReference(TableName);

            var result = await StorageAccessService.GetTableEntityAsync<UserStatsTableEntity>(TableName, PartitionKeyValue, userId);
            result.TotalDrinksLastMonth = count;

            TableOperation saveOperation = TableOperation.InsertOrReplace(result);

            await table.ExecuteAsync(saveOperation);
        }

        public async Task IncreaseScoreAsync(string userId, int additionalScore)
        {
            var table = StorageAccessService.GetTableReference(TableName);

            var result = await StorageAccessService.GetTableEntityAsync<UserStatsTableEntity>(TableName, PartitionKeyValue, userId);
            if(result.Score == null)
            {
                result.Score = 0;
            }
            result.Score += additionalScore;

            TableOperation saveOperation = TableOperation.Replace(result);

            await table.ExecuteAsync(saveOperation);
        }
    }
}
