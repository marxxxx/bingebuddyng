using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity.Domain;
using BingeBuddyNg.Core.Activity.Persistence;
using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Shared;
using Microsoft.WindowsAzure.Storage.Table;

namespace BingeBuddyNg.Core.Activity.Queries
{
    public class GetMasterActivitiesQuery
    {
        private readonly IStorageAccessService storageAccessService;

        public GetMasterActivitiesQuery(IStorageAccessService storageAccessService)
        {
            this.storageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
        }

        public async Task<IEnumerable<ActivityEntity>> ExecuteAsync(ActivityFilterArgs args)
        {
            string whereClause = null;
            string tableName = Constants.TableNames.Activity;

            string currentPartition = ActivityKeyFactory.CreatePartitionKey(DateTime.UtcNow);
            string previousPartition = ActivityKeyFactory.CreatePartitionKey(DateTime.UtcNow.AddDays(-(DateTime.UtcNow.Day + 1)));
            whereClause =
                TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, currentPartition),
                TableOperators.Or,
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, previousPartition));

            if ((args.FilterOptions & ActivityFilterOptions.WithLocation) == ActivityFilterOptions.WithLocation)
            {
                whereClause = TableQuery.CombineFilters(whereClause, TableOperators.And,
                        TableQuery.GenerateFilterConditionForBool(nameof(ActivityTableEntity.HasLocation), QueryComparisons.Equal, true));
            }

            if ((args.FilterOptions & ActivityFilterOptions.WithVenue) == ActivityFilterOptions.WithVenue)
            {
                whereClause = TableQuery.CombineFilters(whereClause, TableOperators.And,
                        TableQuery.GenerateFilterCondition(nameof(ActivityTableEntity.VenueId), QueryComparisons.NotEqual, null));
            }

            if (args.ActivityType != ActivityType.None)
            {
                whereClause = TableQuery.CombineFilters(whereClause, TableOperators.And,
                        TableQuery.GenerateFilterCondition(nameof(ActivityTableEntity.ActivityType), QueryComparisons.Equal, args.ActivityType.ToString()));
            }

            var result = await storageAccessService.QueryTableAsync<JsonTableEntity<ActivityEntity>>(tableName, whereClause, args.PageSize);

            var resultActivities = result.ResultPage.Select(a => a.Entity).ToList();
            return resultActivities;
        }
    }
}
