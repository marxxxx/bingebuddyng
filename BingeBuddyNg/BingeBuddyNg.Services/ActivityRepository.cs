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


        public async Task<List<Activity>> GetActivitysAsync(GetActivityFilterArgs args)
        {
            string currentPartition = GetPartitionKey(DateTime.UtcNow);
            string previousPartition = GetPartitionKey(DateTime.UtcNow.AddDays(-(DateTime.UtcNow.Day + 1)));
            var whereClause =
                TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, currentPartition),
                TableOperators.Or,
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, previousPartition));

            if (args.OnlyWithLocation)
            {
                whereClause = TableQuery.CombineFilters(whereClause, TableOperators.And,
                        TableQuery.GenerateFilterConditionForBool(nameof(ActivityTableEntity.HasLocation), QueryComparisons.Equal, true));
            }

            var result = await StorageAccessService.QueryTableAsync<ActivityTableEntity>(ActivityTableName, whereClause);

            List<Activity> resultActivitys = GetActivitiesWithId(result).OrderByDescending(a => a.Timestamp).ToList();
            return resultActivitys;
        }

        private List<Activity> GetActivitiesWithId(IEnumerable<ActivityTableEntity> result)
        {
            var activitys = result.Select(r => new { r.RowKey, r.Entity }).ToList();
            activitys.ForEach(a => a.Entity.Id = a.RowKey);

            var resultActivitys = activitys.Select(a => a.Entity).ToList();
            return resultActivitys;
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

            var activitys = GetActivitiesWithId(result).OrderBy(a=>a.Timestamp).ToList();
            return activitys;
        }



        public async Task<Activity> AddActivityAsync(Activity activity)
        {
            var activityTable = this.StorageAccessService.GetTableReference(ActivityTableName);

            string rowKey = GetRowKey(activity.Timestamp, activity.UserId);
            var entity = new ActivityTableEntity(GetPartitionKey(activity.Timestamp), rowKey, activity);

            TableOperation operation = TableOperation.Insert(entity);
            await activityTable.ExecuteAsync(operation);

            var perUserActivityTable = this.StorageAccessService.GetTableReference(ActivityPerUserTableName);
                        
            var perUserEntity = new ActivityTableEntity(activity.UserId, rowKey, activity);

            TableOperation perUserOperation = TableOperation.Insert(perUserEntity);
            await perUserActivityTable.ExecuteAsync(perUserOperation);

            activity.Id = rowKey;
            return activity;
        }

        public async Task<Activity> GetActivityAsync(string id)
        {
            ActivityTableEntity entity = await GetActivityEntity(id);

            return entity.Entity;
        }

        private async Task<ActivityTableEntity> GetActivityEntity(string id)
        {
            string partitionKey = GetPartitionKey(id);

            var table = this.StorageAccessService.GetTableReference(ActivityTableName);

            TableOperation retrieveOperation = TableOperation.Retrieve<ActivityTableEntity>(partitionKey, id);

            var result = await table.ExecuteAsync(retrieveOperation);

            var entity = (ActivityTableEntity)result.Result;
            entity.Entity.Id = id;
            return entity;
        }

        public async Task UpdateActivityAsync(Activity activity)
        {
            var table = this.StorageAccessService.GetTableReference(ActivityTableName);

            ActivityTableEntity entity = await GetActivityEntity(activity.Id);
            
            // extend to other propertys if needed
            entity.Entity.LocationAddress = activity.LocationAddress;
            entity.Entity.Likes = activity.Likes;
            entity.Entity.Comments = activity.Comments;
            entity.Entity.Cheers = activity.Cheers;

            TableOperation updateOperation = TableOperation.InsertOrReplace(entity);
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
