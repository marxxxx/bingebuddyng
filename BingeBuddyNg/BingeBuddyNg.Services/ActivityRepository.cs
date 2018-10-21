﻿using BingeBuddyNg.Services.DTO;
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
        private static readonly DateTime MaxTimestamp = new DateTime(2100, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public StorageAccessService StorageAccessService { get; }

        public ActivityRepository(StorageAccessService storageAccessService)
        {
            this.StorageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
        }


        public async Task<PagedQueryResult<Activity>> GetActivityFeedAsync(GetActivityFilterArgs args)
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

          

            var result = await StorageAccessService.QueryTableAsync<ActivityTableEntity>(ActivityTableName, whereClause, args.PageSize, args.ContinuationToken);

            List<Activity> resultActivitys = GetActivitiesWithId(result.ResultPage).ToList();
            return new PagedQueryResult<Activity>(resultActivitys, result.ContinuationToken);
        }

        private List<Activity> GetActivitiesWithId(IEnumerable<ActivityTableEntity> result)
        {
            List<Activity> resultActivities = new List<Activity>();
            foreach(var r in result)
            {
                r.Entity.Id = r.RowKey;
                resultActivities.Add(r.Entity);
            }
            return resultActivities;
        }

        public async Task<List<Activity>> GetActivitysForUser(string userId, DateTime startTimeUtc, ActivityType activityType)
        {
            var whereClause =
                TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userId),
                TableOperators.And,
                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, GetActivityPerUserRowKey(startTimeUtc)));

            whereClause = TableQuery.CombineFilters(whereClause, TableOperators.And,
                TableQuery.GenerateFilterCondition(nameof(ActivityTableEntity.ActivityType), QueryComparisons.Equal, activityType.ToString()));


            var result = await StorageAccessService.QueryTableAsync<ActivityTableEntity>(ActivityPerUserTableName, whereClause);

            var activitys = GetActivitiesWithId(result).ToList();
            return activitys;
        }



        public async Task<Activity> AddActivityAsync(Activity activity)
        {
            var activityTable = this.StorageAccessService.GetTableReference(ActivityTableName);

            string activityFeedRowKey = GetActivityFeedRowKey(activity.Timestamp, activity.UserId);
            var entity = new ActivityTableEntity(GetPartitionKey(activity.Timestamp), activityFeedRowKey, activity);

            TableOperation operation = TableOperation.Insert(entity);
            await activityTable.ExecuteAsync(operation);

            var perUserActivityTable = this.StorageAccessService.GetTableReference(ActivityPerUserTableName);

            string activityPerUserRowKey = GetActivityPerUserRowKey(activity.Timestamp);
            var perUserEntity = new ActivityTableEntity(activity.UserId, activityPerUserRowKey, activity);

            TableOperation perUserOperation = TableOperation.Insert(perUserEntity);
            await perUserActivityTable.ExecuteAsync(perUserOperation);

            activity.Id = activityFeedRowKey;
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

            TableOperation updateOperation = TableOperation.Replace(entity);
            await table.ExecuteAsync(updateOperation);
        }

        private string GetPartitionKey(DateTime timestampUtc)
        {
            return $"{MaxTimestamp.Year- timestampUtc.Year}-{12-timestampUtc.Month}";
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
