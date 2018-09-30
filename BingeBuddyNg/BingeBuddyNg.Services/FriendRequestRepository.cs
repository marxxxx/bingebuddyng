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
    public class FriendRequestRepository : IFriendRequestRepository
    {
        private const string TableName = "friendrequests";

        public StorageAccessService StorageAccesService { get; }

        public FriendRequestRepository(StorageAccessService storageAccessService)
        {
            this.StorageAccesService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
        }

        public async Task AddFriendRequestAsync(UserInfo friend, UserInfo requestingUser)
        {
            var table = StorageAccesService.GetTableReference(TableName);
            
            var requestingEntity = new FriendRequestEntity(friend.UserId, requestingUser.UserId, requestingUser, friend);
            var friendEntity = new FriendRequestEntity(requestingUser.UserId, friend.UserId, requestingUser, friend);

            await table.ExecuteAsync(TableOperation.Insert(requestingEntity));
            await table.ExecuteAsync(TableOperation.Insert(friendEntity));
        }


        public async Task DeleteFriendRequestAsync(string userId, string requestingUserId)
        {
            await this.StorageAccesService.DeleteTableEntityAsync(TableName, userId, requestingUserId);
            await this.StorageAccesService.DeleteTableEntityAsync(TableName, requestingUserId, userId);
        }


        public async Task<List<FriendRequestInfo>> GetFriendRequestsAsync(string userId)
        {
            var whereClause = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userId);
            
            var queryResult = await this.StorageAccesService.QueryTableAsync<FriendRequestEntity>(TableName, whereClause);

            var result = queryResult.Select(r => new FriendRequestInfo(
                new UserInfo(r.RequestingUserId, r.RequestingUserName, r.RequestingUserProfileImageUrl),
                new UserInfo(r.FriendUserId, r.FriendUserName, r.FriendUserProfileImageUrl))).ToList();
            return result;
        }

        public async Task<bool> HasPendingFriendRequestAsync(string userId, string requestingUserId)
        {
            var result = await Task.WhenAll(InternalHasPendingFriendRequestAsync(userId, requestingUserId), InternalHasPendingFriendRequestAsync(requestingUserId, userId));

            return result.Any(r=>r);
        }

        private async Task<bool> InternalHasPendingFriendRequestAsync(string userId, string requestingUserId)
        {
            var whereClause = TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userId),
                TableOperators.And,
                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, requestingUserId));

            var queryResult = await this.StorageAccesService.QueryTableAsync<FriendRequestEntity>(TableName, whereClause);

            return queryResult.Any();
        }
    }
}
