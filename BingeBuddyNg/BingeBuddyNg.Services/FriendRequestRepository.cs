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

        public async Task AddFriendRequestAsync(string userId, UserInfo requestingUser)
        {
            var table = StorageAccesService.GetTableReference(TableName);

            var entity = new FriendRequestEntity(userId, DateTime.UtcNow, requestingUser.UserId,
                requestingUser.UserName, requestingUser.UserProfileImageUrl);
            TableOperation operation = TableOperation.Insert(entity);

            await table.ExecuteAsync(operation);
        }


        public async Task DeleteFriendRequestAsync(string userId, string requestingUserId)
        {
            await this.StorageAccesService.DeleteTableEntityAsync(TableName, userId, requestingUserId);
        }


        public async Task<List<UserInfo>> GetFriendRequestsAsync(string userId)
        {
            var whereClause = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userId);

            var queryResult = await this.StorageAccesService.QueryTableAsync<FriendRequestEntity>(TableName, whereClause);

            var result = queryResult.Select(r => new UserInfo(r.RequestingUserId, r.RequestingUserName, r.RequestingUserProfileImageUrl)).ToList();
            return result;
        }

        public async Task<bool> HasPendingFriendRequestAsync(string userId, string requestingUserId)
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
