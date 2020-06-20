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

        public async Task<User> GetUserAsync(string id)
        {
            var result = await FindUserEntityAsync(id);
            if (result?.Entity == null)
            {
                throw new NotFoundException($"User {id} not found!");
            }

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
