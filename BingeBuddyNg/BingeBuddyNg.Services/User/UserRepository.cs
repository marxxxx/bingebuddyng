using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.User.Commands;
using BingeBuddyNg.Services.User.Persistence;
using BingeBuddyNg.Services.Venue;
using Microsoft.WindowsAzure.Storage.Table;

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
            UserEntity user = null;
            if (result != null)
            {
                user = result.Entity;
                user.LastOnline = result.Timestamp.UtcDateTime;
            }

            return new User(user.Id, user.Name, user.Weight, user.Gender, user.ProfileImageUrl, user.PushInfo, user.Friends, user.MutedByFriendUserIds, user.MutedByFriendUserIds, user.MonitoringInstanceId, user.CurrentVenue?.ToDomain(), user.Language, user.LastOnline);
        }

        private async Task<JsonTableEntity<UserEntity>> FindUserEntityAsync(string id)
        {
            var result = await cacheService.GetOrCreateAsync<JsonTableEntity<UserEntity>>(GetUserCacheKey(id), async () =>
            {
                var table = storageAccess.GetTableReference(TableName);

                TableOperation retrieveOperation = TableOperation.Retrieve<JsonTableEntity<User>>(PartitionKeyValue, id);

                var userResult = await table.ExecuteAsync(retrieveOperation);

                return userResult?.Result as JsonTableEntity<UserEntity>;
            }, TimeSpan.FromMinutes(1));

            return result;
        }

        private string GetUserCacheKey(string userId) => $"User:{userId}";


        public async Task<CreateOrUpdateUserResult> CreateOrUpdateUserAsync(CreateOrUpdateUserCommand request)
        {
            var table = storageAccess.GetTableReference(TableName);
            bool profilePicHasChanged = true;
            bool nameHasChanged = false;
            var savedUser = await FindUserEntityAsync(request.UserId);
            bool isNewUser = false;
            string originalUserName = null;

            TableOperation saveUserOperation;
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

                if (request.Language != null)
                {
                    savedUser.Entity.Language = request.Language;
                }

                saveUserOperation = TableOperation.Replace(savedUser);

                cacheService.Remove(GetUserCacheKey(request.UserId));
            }
            else
            {
                var user = new UserEntity()
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
                saveUserOperation = TableOperation.Insert(new JsonTableEntity<UserEntity>(PartitionKeyValue, request.UserId, user));
                isNewUser = true;
            }

            await table.ExecuteAsync(saveUserOperation);

            return new CreateOrUpdateUserResult(isNewUser, profilePicHasChanged, nameHasChanged, originalUserName);
        }

        public async Task UpdateUserAsync(UserEntity user)
        {            
            var userEntity = await FindUserEntityAsync(user.Id);
            userEntity.Entity = user;

            var table = storageAccess.GetTableReference(TableName);
            TableOperation saveUserOperation = TableOperation.Replace(userEntity);

            await table.ExecuteAsync(saveUserOperation);

            cacheService.Remove(GetUserCacheKey(user.Id));
        }

        public async Task<IEnumerable<UserEntity>> GetUsersAsync(IEnumerable<string> userIds = null)
        {
            string whereClause = BuildWhereClause(userIds);

            var result = await storageAccess.QueryTableAsync<JsonTableEntity<UserEntity>>(TableName, whereClause);

            var users = result.OrderByDescending(u => u.Timestamp).Select(r =>
              {
                  var user = r.Entity;
                  user.LastOnline = r.Timestamp.UtcDateTime;
                  return user;
              }).ToList();
            return users;
        }

        public async Task<IEnumerable<string>> GetAllUserIdsAsync()
        {
            return await this.storageAccess.GetRowKeysAsync(TableName, PartitionKeyValue);
        }

        private string BuildWhereClause(IEnumerable<string> userIds)
        {
            string whereClause = null;

            if (userIds != null)
            {
                foreach (var u in userIds)
                {
                    string filter = TableQuery.GenerateFilterCondition(nameof(TableEntity.RowKey), QueryComparisons.Equal, u);
                    if (whereClause != null)
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

     
        public async Task UpdateMonitoringInstanceAsync(string userId, string monitoringInstanceId)
        {
            var user = await FindUserEntityAsync(userId);
            if (user == null || user.Entity.MonitoringInstanceId == monitoringInstanceId)
            {
                return;
            }

            var table = storageAccess.GetTableReference(TableName);

            user.Entity.MonitoringInstanceId = monitoringInstanceId;
            TableOperation saveUserOperation = TableOperation.Replace(user);

            await table.ExecuteAsync(saveUserOperation);
        }
    }
}
