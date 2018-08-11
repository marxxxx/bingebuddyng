using BingeBuddyNg.Services.DTO;
using BingeBuddyNg.Services.Entitys;
using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Models;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services
{
    public class ActivityRepository : IActivityRepository
    {
        private const string TableName = "activity";

        public StorageAccessService StorageAccessService { get; }

        public ActivityRepository(StorageAccessService storageAccessService)
        {
            this.StorageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
        }


        public async Task<List<Activity>> GetActivitysAsync()
        {
            string currentPartition = DateTime.UtcNow.ToString("yyyyMM");
            string previousPartition = DateTime.UtcNow.AddDays(-(DateTime.UtcNow.Day+1)).ToString("yyyyMM");
            var whereClause =
                TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, currentPartition),
                TableOperators.Or,
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, previousPartition));


            var result = await StorageAccessService.QueryTableAsync<ActivityTableEntity>(TableName, whereClause);

            var activitys = result.Select(r => EntityConverters.Activitys.EntityToModel(r)).ToList();
            return activitys;
        }

        public Task AddActivityAsync(Activity activity)
        {
            var table = this.StorageAccessService.GetTableReference(TableName);

            var entity = EntityConverters.Activitys.ModelToEntity(activity);

            TableOperation operation = TableOperation.Insert(entity);
            return table.ExecuteAsync(operation);
        }

    }
}
