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
        private const string ActivityTableName = "activity";
        private const string ActivityPerUserTableName = "activityperuser";


        public StorageAccessService StorageAccessService { get; }

        public ActivityRepository(StorageAccessService storageAccessService)
        {
            this.StorageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
        }


        public async Task<List<Activity>> GetActivitysAsync()
        {
            string currentPartition = GetPartitionKey(DateTime.UtcNow);
            string previousPartition = GetPartitionKey(DateTime.UtcNow.AddDays(-(DateTime.UtcNow.Day + 1)));
            var whereClause =
                TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, currentPartition),
                TableOperators.Or,
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, previousPartition));


            var result = await StorageAccessService.QueryTableAsync<ActivityTableEntity>(ActivityTableName, whereClause);

            var activitys = result.Select(r => EntityConverters.Activitys.EntityToModel(r)).OrderByDescending(a=>a.Timestamp).ToList();
            return activitys;
        }

        public async Task<List<Activity>> GetActivitysForUser(string userId, DateTime startTimeUtc, ActivityType activityType)
        {
            var whereClause =
                TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userId),
                TableOperators.And,
                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, GetRowKey(startTimeUtc, userId)));

            whereClause = TableQuery.CombineFilters(whereClause, TableOperators.And,
                TableQuery.GenerateFilterCondition(nameof(ActivityTableEntity.ActivityType), QueryComparisons.Equal, activityType.ToString()));


            var result = await StorageAccessService.QueryTableAsync<ActivityTableEntity>(ActivityPerUserTableName, whereClause);

            var activitys = result.Select(r => EntityConverters.Activitys.EntityToModel(r)).OrderBy(a=>a.Timestamp).ToList();
            return activitys;
        }



        public async Task<Activity> AddActivityAsync(Activity activity)
        {
            var activityTable = this.StorageAccessService.GetTableReference(ActivityTableName);

            string rowKey = GetRowKey(activity.Timestamp, activity.UserId);
            var entity = EntityConverters.Activitys.ModelToEntity(activity, GetPartitionKey(activity.Timestamp), rowKey);

            TableOperation operation = TableOperation.Insert(entity);
            await activityTable.ExecuteAsync(operation);

            var perUserActivityTable = this.StorageAccessService.GetTableReference(ActivityPerUserTableName);
                        
            var perUserEntity = EntityConverters.Activitys.ModelToEntity(activity, activity.UserId, rowKey);

            TableOperation perUserOperation = TableOperation.Insert(perUserEntity);
            await perUserActivityTable.ExecuteAsync(perUserOperation);

            activity.Id = rowKey;
            return activity;
        }

        public async Task<Activity> GetActivityAsync(string userId, string id)
        {
            ActivityTableEntity entity = await GetActivityEntity(id);

            var activity = EntityConverters.Activitys.EntityToModel(entity);
            return activity;
        }

        private async Task<ActivityTableEntity> GetActivityEntity(string id)
        {
            string partitionKey = GetPartitionKey(id);

            var table = this.StorageAccessService.GetTableReference(ActivityTableName);

            TableOperation retrieveOperation = TableOperation.Retrieve<ActivityTableEntity>(partitionKey, id);

            var result = await table.ExecuteAsync(retrieveOperation);

            var entity = (ActivityTableEntity)result.Result;
            return entity;
        }

        public async Task UpdateActivityAsync(Activity activity)
        {
            var table = this.StorageAccessService.GetTableReference(ActivityTableName);

            ActivityTableEntity entity = await GetActivityEntity(activity.Id);
            
            // extend to other propertys if needed
            entity.LocationAddress = activity.LocationAddress;

            TableOperation updateOperation = TableOperation.InsertOrMerge(entity);
            await table.ExecuteAsync(updateOperation);
        }

        private string GetPartitionKey(DateTime timestampUtc)
        {
            return timestampUtc.ToString("yyyyMM");
        }

        private string GetPartitionKey(string rowKey)
        {
            string partitionKey = rowKey.Substring(0, 6);
            return partitionKey;
        }

        private string GetRowKey(DateTime timestampUtc, string userId)
        {
            return timestampUtc.ToString("yyyyMMddHHmmss") + "|" + userId;
        }
    }
}
