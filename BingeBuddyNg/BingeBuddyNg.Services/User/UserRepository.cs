using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.User.Commands;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.User
{
    public class UserRepository : IUserRepository
    {
        private const string TableName = "users";
        private const string PartitionKeyValue = "User";

        private readonly IStorageAccessService storageAccess;
        private readonly ICacheService cacheService;

        public UserRepository(IStorageAccessService storageAccess, ICacheService cacheService)
        {
            this.storageAccess = storageAccess ?? throw new ArgumentNullException(nameof(storageAccess));
            this.cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }                

        public async Task<User> FindUserAsync(string id)
        {
            var result = await FindUserEntityAsync(id);
            User user = null;
            if (result != null)
            {
                user = result.Entity;
                user.LastOnline = result.Timestamp.UtcDateTime;
            }

            return user;
        }

        private async Task<JsonTableEntity<User>> FindUserEntityAsync(string id)
        {
            var result = await cacheService.GetOrCreateAsync<JsonTableEntity<User>>(GetUserCacheKey(id), async () =>
            {
                var table = storageAccess.GetTableReference(TableName);

                TableOperation retrieveOperation = TableOperation.Retrieve<JsonTableEntity<User>>(PartitionKeyValue, id);

                var userResult = await table.ExecuteAsync(retrieveOperation);

                return userResult?.Result as JsonTableEntity<User>;
            }, TimeSpan.FromMinutes(1));

            return result;
        }

        private string GetUserCacheKey(string userId) => $"User:{userId}";
        

        public async Task<CreateOrUpdateUserResult> CreateOrUpdateUserAsync(CreateOrUpdateUserCommand request)
        {
            var table = storageAccess.GetTableReference(TableName);

            TableOperation saveUserOperation = null;
            bool profilePicHasChanged = true;
            bool nameHasChanged = false;
            var savedUser = await FindUserEntityAsync(request.UserId);
            bool isNewUser = false;
            string originalUserName = null;
            if (savedUser != null)
            {
                originalUserName = savedUser.Entity.Name;

                profilePicHasChanged = savedUser.Entity.ProfileImageUrl != request.ProfileImageUrl;
                nameHasChanged = savedUser.Entity.Name != request.Name;

                savedUser.Entity.Name = request.Name;
                savedUser.Entity.ProfileImageUrl = request.ProfileImageUrl;
                if (request.PushInfo != null && request.PushInfo.HasValue())
                {
                    savedUser.Entity.PushInfo = request.PushInfo;
                }

                if(request.Language != null)
                {
                    savedUser.Entity.Language = request.Language;
                }

                saveUserOperation = TableOperation.Replace(savedUser);

                cacheService.Remove(GetUserCacheKey(request.UserId));
            }
            else
            {
                var user = new User()
                {
                    Id = request.UserId,
                    Name = request.Name,
                    Gender = Gender.Male,
                    Language = request.Language,
                    PushInfo = request.PushInfo,
                    Weight = 80,
                    LastOnline = DateTime.UtcNow,
                    ProfileImageUrl = request.ProfileImageUrl
                };
                saveUserOperation = TableOperation.Insert(new JsonTableEntity<User>(PartitionKeyValue, request.UserId, user));
                isNewUser = true;
            }

            await table.ExecuteAsync(saveUserOperation);

            return new CreateOrUpdateUserResult(isNewUser, profilePicHasChanged, nameHasChanged, originalUserName);
        }

        public async Task UpdateUserAsync(User user)
        {
            var table = storageAccess.GetTableReference(TableName);
            var userEntity = await FindUserEntityAsync(user.Id);
            userEntity.Entity = user;
            
            TableOperation saveUserOperation = TableOperation.Replace(userEntity);

            await table.ExecuteAsync(saveUserOperation);

            cacheService.Remove(GetUserCacheKey(user.Id));
        }

        public async Task<List<User>> GetUsersAsync(IEnumerable<string> userIds = null)
        {
            string whereClause = BuildWhereClause(userIds);

            var result = await storageAccess.QueryTableAsync<JsonTableEntity<User>>(TableName, whereClause);

            var users = result.OrderByDescending(u=>u.Timestamp).Select(r =>
            {
                var user = r.Entity;
                user.LastOnline = r.Timestamp.UtcDateTime;
                return user;
            }).ToList();
            return users;
        }

        private string BuildWhereClause(IEnumerable<string> userIds)
        {
            string whereClause = null;

            if(userIds != null)
            {
                foreach(var u in userIds)
                {
                    string filter = TableQuery.GenerateFilterCondition(nameof(TableEntity.RowKey), QueryComparisons.Equal, u);
                    if(whereClause != null)
                    {
                        whereClause = TableQuery.CombineFilters(whereClause, TableOperators.Or, filter);
                    }
                    else
                    {
                        whereClause = filter;
                    }
                }
            }

            return whereClause;
        }

        public async Task AddFriendAsync(string userId, string friendUserId)
        {
            var user = await FindUserAsync(userId);
            var friend = await FindUserAsync(friendUserId);

            user.AddFriend(friend.ToUserInfo());
            friend.AddFriend(user.ToUserInfo());

            await Task.WhenAll(UpdateUserAsync(user), UpdateUserAsync(friend));
        }

        public async Task RemoveFriendAsync(string userId, string friendUserId)
        {
            var results = await Task.WhenAll(FindUserAsync(userId), FindUserAsync(friendUserId));

            results[0].RemoveFriend(friendUserId);
            results[1].RemoveFriend(userId);

            await Task.WhenAll(UpdateUserAsync(results[0]), UpdateUserAsync(results[1]));
        }

        public async Task UpdateMonitoringInstanceAsync(string userId, string monitoringInstanceId)
        {
            var user = await FindUserEntityAsync(userId);
            if(user != null)
            {
                user.Entity.MonitoringInstanceId = monitoringInstanceId;
            }
        }
    }
}
