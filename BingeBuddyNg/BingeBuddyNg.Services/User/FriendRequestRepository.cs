using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Core.FriendsRequest.Persistence;
using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Core.User.Persistence;
using Microsoft.WindowsAzure.Storage.Table;

namespace BingeBuddyNg.Core.User
{
    public class FriendRequestRepository : IFriendRequestRepository
    {
        private const string TableName = "friendrequests";

        private readonly IStorageAccessService storageAccessService;

        public FriendRequestRepository(IStorageAccessService storageAccessService)
        {
            this.storageAccessService = storageAccessService;
        }

        public async Task AddFriendRequestAsync(UserInfo friend, UserInfo requestingUser)
        {
            var requestingEntity = new FriendRequestEntity(friend.UserId, requestingUser.UserId, requestingUser, friend);
            var friendEntity = new FriendRequestEntity(requestingUser.UserId, friend.UserId, requestingUser, friend);

            await storageAccessService.InsertAsync(TableName, requestingEntity);
            await storageAccessService.InsertAsync(TableName, friendEntity);
        }

        public async Task DeleteFriendRequestAsync(string userId, string requestingUserId)
        {
            await this.storageAccessService.DeleteAsync(TableName, userId, requestingUserId);
            await this.storageAccessService.DeleteAsync(TableName, requestingUserId, userId);
        }

        public async Task<List<FriendRequestEntity>> GetFriendRequestsAsync(string userId)
        {
            var queryResult = await this.storageAccessService.QueryTableAsync<FriendRequestEntity>(TableName, partitionKey: userId, minRowKey: null, pageSize: 50);

            var result = queryResult.ResultPage;
            return result;
        }

        public async Task<bool> HasPendingFriendRequestAsync(string userId, string requestingUserId)
        {
            var result = await Task.WhenAll(InternalHasPendingFriendRequestAsync(userId, requestingUserId), InternalHasPendingFriendRequestAsync(requestingUserId, userId));

            return result.Any(r => r);
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
