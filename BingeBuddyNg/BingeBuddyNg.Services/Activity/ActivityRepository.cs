using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Shared;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace BingeBuddyNg.Services.Activity
{
    public class ActivityRepository : IActivityRepository
    {
        private const string ActivityTableName = "activity";
        private const string ActivityPerUserTableName = "activityperuser";
        private static readonly DateTime MaxTimestamp = new DateTime(2100, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private readonly IStorageAccessService storageAccessService;
        private readonly ICacheService sacheService;

        public ActivityRepository(IStorageAccessService storageAccessService, ICacheService cacheService)
        {
            this.storageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
            this.sacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }
        
        public string GetActivityCacheKey(string userId) => $"Activity:{userId}";

        public async Task<PagedQueryResult<Activity>> GetActivityFeedAsync(GetActivityFilterArgs args)
        {
            string currentPartition = GetPartitionKey(DateTime.UtcNow);
            string previousPartition = GetPartitionKey(DateTime.UtcNow.AddDays(-(DateTime.UtcNow.Day + 1)));
            var whereClause =
                TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, currentPartition),
                TableOperators.Or,
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, previousPartition));

            if(string.IsNullOrEmpty(args.StartActivityId) == false)
            {
                whereClause = TableQuery.CombineFilters(whereClause, TableOperators.And,
                        TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, args.StartActivityId));
            }

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

            if (args.UserIds != null && args.UserIds.Any())
            {
                string userWhereClause = null;
                foreach (var userId in args.UserIds)
                {
                    var userCondition = TableQuery.GenerateFilterCondition(nameof(ActivityTableEntity.UserId), QueryComparisons.Equal, userId);
                    if (userWhereClause == null)
                    {
                        userWhereClause = userCondition;
                    }
                    else
                    {
                        userWhereClause = TableQuery.CombineFilters(userWhereClause, TableOperators.Or, userCondition);
                    }
                }

                whereClause = TableQuery.CombineFilters(whereClause, TableOperators.And, userWhereClause);
            }



            var result = await storageAccessService.QueryTableAsync<ActivityTableEntity>(ActivityTableName, whereClause, args.PageSize, args.ContinuationToken);

            List<Activity> resultActivitys = ConvertActivities(result.ResultPage).ToList();
            return new PagedQueryResult<Activity>(resultActivitys, result.ContinuationToken);
        }


        private List<Activity> ConvertActivities(IEnumerable<ActivityTableEntity> result)
        {
            List<Activity> resultActivities = new List<Activity>();
            foreach(var r in result)
            {
                r.Entity.Id = r.RowKey;
                resultActivities.Add(r.Entity);
            }
            return resultActivities;
        }

        public async Task<List<Activity>> GetActivitysForUserAsync(string userId, DateTime startTimeUtc, ActivityType activityType)
        {
            string startRowKey = GetActivityPerUserRowKey(startTimeUtc);
            var whereClause =
                TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userId),
                TableOperators.And,
                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, startRowKey));

            whereClause = TableQuery.CombineFilters(whereClause, TableOperators.And,
                TableQuery.GenerateFilterCondition(nameof(ActivityTableEntity.ActivityType), QueryComparisons.Equal, activityType.ToString()));


            var result = await storageAccessService.QueryTableAsync<ActivityTableEntity>(ActivityPerUserTableName, whereClause);

            var activitys = ConvertActivities(result).ToList();
            return activitys;
        }

        public async Task<Activity> AddActivityAsync(Activity activity)
        {
            var activityTable = this.storageAccessService.GetTableReference(ActivityTableName);

            string activityFeedRowKey = GetActivityFeedRowKey(activity.Timestamp, activity.UserId);
            activity.Id = activityFeedRowKey;
            var entity = new ActivityTableEntity(GetPartitionKey(activity.Timestamp), activityFeedRowKey, activity);

            TableOperation operation = TableOperation.Insert(entity);
            await activityTable.ExecuteAsync(operation);

            var perUserActivityTable = this.storageAccessService.GetTableReference(ActivityPerUserTableName);

            string activityPerUserRowKey = GetActivityPerUserRowKey(activity.Timestamp);
            var perUserEntity = new ActivityTableEntity(activity.UserId, activityPerUserRowKey, activity);

            TableOperation perUserOperation = TableOperation.Insert(perUserEntity);
            await perUserActivityTable.ExecuteAsync(perUserOperation);

            sacheService.Remove(GetActivityCacheKey(activity.UserId));

            return activity;
        }

        public async Task<Activity> GetActivityAsync(string id)
        {
            ActivityTableEntity entity = await GetActivityEntityAsync(id);

            return entity.Entity;
        }

        private async Task<ActivityTableEntity> GetActivityEntityAsync(string id)
        {
            string partitionKey = GetPartitionKey(id);
            var table = this.storageAccessService.GetTableReference(ActivityTableName);

            TableOperation retrieveOperation = TableOperation.Retrieve<ActivityTableEntity>(partitionKey, id);

            var result = await table.ExecuteAsync(retrieveOperation);

            var entity = (ActivityTableEntity)result.Result;
            entity.Entity.Id = id;
            return entity;
        }

        private async Task<ActivityTableEntity> GetActivityPerUserEntityAsync(string userId, DateTime timestamp)
        {
            var table = this.storageAccessService.GetTableReference(ActivityPerUserTableName);
            var rowKey = GetActivityPerUserRowKey(timestamp);

            TableOperation retrieveOperation = TableOperation.Retrieve<ActivityTableEntity>(userId, rowKey);

            var result = await table.ExecuteAsync(retrieveOperation);

            var entity = (ActivityTableEntity)result.Result;
            return entity;
        }

        public async Task UpdateActivityAsync(Activity activity)
        {
            var table = this.storageAccessService.GetTableReference(ActivityTableName);

            ActivityTableEntity entity = await GetActivityEntityAsync(activity.Id);
            
            // extend to other propertys if needed
            // Note to my future-self: Why do we need this? Just replace entity maybe and we're good?
            entity.Entity.LocationAddress = activity.LocationAddress;
            entity.Entity.Likes = activity.Likes;
            entity.Entity.Comments = activity.Comments;
            entity.Entity.Cheers = activity.Cheers;
            entity.Entity.DrinkCount = activity.DrinkCount;
            entity.Entity.AlcLevel = activity.AlcLevel;

            TableOperation updateOperation = TableOperation.Replace(entity);
            await table.ExecuteAsync(updateOperation);

            // invalidate cache
            sacheService.Remove(GetActivityCacheKey(activity.UserId));
        }

        public async Task DeleteActivityAsync(string userId, string id)
        {
            var activityTable = this.storageAccessService.GetTableReference(ActivityTableName);
            var activity = await this.GetActivityEntityAsync(id);
            if(string.Compare(activity.UserId,  userId, true) != 0)
            {
                throw new UnauthorizedAccessException($"User {userId} is not permitted to delete an activity of user {activity.UserId}");
            }

            await activityTable.ExecuteAsync(TableOperation.Delete(activity));

            // Delete activity in per-user table as well
            var perUserTable = this.storageAccessService.GetTableReference(ActivityPerUserTableName);
            var perUserActivity = await this.GetActivityPerUserEntityAsync(userId, activity.Entity.Timestamp);
            await perUserTable.ExecuteAsync(TableOperation.Delete(perUserActivity));

            sacheService.Remove(GetActivityCacheKey(userId));
        }


        public async Task AddToActivityAddedQueueAsync(string activityId)
        {
            var queueClient = this.storageAccessService.GetQueueReference(Constants.QueueNames.ActivityAdded);
            var message = new ActivityAddedMessage(activityId);
            await queueClient.AddMessageAsync(new Microsoft.WindowsAzure.Storage.Queue.CloudQueueMessage(JsonConvert.SerializeObject(message)));
        }

        private string GetPartitionKey(DateTime timestampUtc)
        {
            int year = MaxTimestamp.Year - timestampUtc.Year;
            int month = 12 - timestampUtc.Month;
            return string.Format("{0:D2}-{1:D2}", year, month);
        }

        private string GetPartitionKey(string rowKey)
        {
            string[] tokens = rowKey.Split('|');
            return tokens[0];
        }


        private string GetActivityPerUserRowKey(DateTime timestampUtc)
        {
            return timestampUtc.ToString("yyyyMMddHHmmss");
        }

        private string GetActivityFeedRowKey(DateTime timestampUtc, string userId)
        {
            long ticks = (MaxTimestamp - timestampUtc).Ticks;
            string partitionKey = GetPartitionKey(timestampUtc);

            return $"{partitionKey}|{ticks}|{userId}";
        }

    }
}
