using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.User;
using Microsoft.WindowsAzure.Storage.Table;

namespace BingeBuddyNg.Services.FriendsRequest
{
    public class FriendRequestRepository : IFriendRequestRepository
    {
        private const string TableName = "friendrequests";

        private readonly IStorageAccessService storageAccessService;

        public FriendRequestRepository(IStorageAccessService storageAccessService)
        {
            this.storageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
        }

        public async Task AddFriendRequestAsync(UserInfo friend, UserInfo requestingUser)
        {
            var table = storageAccessService.GetTableReference(TableName);
            
            var requestingEntity = new FriendRequestEntity(friend.UserId, requestingUser.UserId, requestingUser, friend);
            var friendEntity = new FriendRequestEntity(requestingUser.UserId, friend.UserId, requestingUser, friend);

            await table.ExecuteAsync(TableOperation.Insert(requestingEntity));
            await table.ExecuteAsync(TableOperation.Insert(friendEntity));
        }


        public async Task DeleteFriendRequestAsync(string userId, string requestingUserId)
        {
            await this.storageAccessService.DeleteTableEntityAsync(TableName, userId, requestingUserId);
            await this.storageAccessService.DeleteTableEntityAsync(TableName, requestingUserId, userId);
        }


        public async Task<List<FriendRequestDTO>> GetFriendRequestsAsync(string userId)
        {
            var whereClause = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userId);
            
            var queryResult = await this.storageAccessService.QueryTableAsync<FriendRequestEntity>(TableName, whereClause);

            var result = queryResult.Select(r => new FriendRequestDTO(
                new UserInfoDTO(r.RequestingUserId, r.RequestingUserName),
                new UserInfoDTO(r.FriendUserId, r.FriendUserName))).ToList();
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

            var queryResult = await this.storageAccessService.QueryTableAsync<FriendRequestEntity>(TableName, whereClause);

            return queryResult.Any();
        }
    }
}
