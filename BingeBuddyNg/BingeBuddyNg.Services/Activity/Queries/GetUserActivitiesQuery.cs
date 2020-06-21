using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity.Domain;
using BingeBuddyNg.Core.Activity.Persistence;
using BingeBuddyNg.Services.Activity.Persistence;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Shared;
using Microsoft.WindowsAzure.Storage.Table;

namespace BingeBuddyNg.Core.Activity.Queries
{
    public class GetUserActivitiesQuery
    {
        private readonly IStorageAccessService storageAccessService;

        public GetUserActivitiesQuery(IStorageAccessService storageAccessService)
        {
            this.storageAccessService = storageAccessService;
        }

        public async Task<IEnumerable<ActivityEntity>> ExecuteAsync(string userId, DateTime startTimeUtc, ActivityType activityType = ActivityType.None)
        {
            string startRowKey = ActivityKeyFactory.CreatePerUserRowKey(startTimeUtc);
            var whereClause =
                TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userId),
                TableOperators.And,
                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, startRowKey));

            if (activityType != Core.Activity.Domain.ActivityType.None)
            {
                whereClause = TableQuery.CombineFilters(whereClause, TableOperators.And,
                    TableQuery.GenerateFilterCondition(nameof(ActivityTableEntity.ActivityType), QueryComparisons.Equal, activityType.ToString()));
            }

            var result = await storageAccessService.QueryTableAsync<JsonTableEntity<ActivityEntity>>(Constants.TableNames.ActivityPerUser, whereClause);

            var activitys = result.Select(a => a.Entity).ToList();
            return activitys;
        }
    }
}
