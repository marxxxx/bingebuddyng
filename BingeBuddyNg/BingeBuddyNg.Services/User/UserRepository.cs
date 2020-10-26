using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Core.User.Commands;
using BingeBuddyNg.Core.User.Domain;
using BingeBuddyNg.Core.User.Persistence;
using BingeBuddyNg.Core.Venue;
using Microsoft.WindowsAzure.Storage.Table;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Core.User
{
    public class UserRepository : IUserRepository
    {
        private readonly IStorageAccessService storageAccessService;
        private readonly ICacheService cacheService;

        public UserRepository(IStorageAccessService storageAccess, ICacheService cacheService)
        {
            this.storageAccessService = storageAccess ?? throw new ArgumentNullException(nameof(storageAccess));
            this.cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        public async Task<Domain.User> GetUserAsync(string id)
        {
            var result = await FindUserEntityAsync(id);
            if (result?.Entity == null)
            {
                throw new NotFoundException($"User {id} not found!");
            }

            UserEntity entity = result.Entity;

            return new Domain.User(entity.Id, entity.Name, entity.Weight, entity.Gender, entity.ProfileImageUrl, entity.PushInfo, entity.Friends, entity.CurrentVenue?.ToDomain(), entity.Language, entity.LastOnline, entity.PendingFriendRequests, entity.Invitations);
        }

        private async Task<JsonTableEntity<UserEntity>> FindUserEntityAsync(string id)
        {
            var result = await cacheService.GetOrCreateAsync(GetUserCacheKey(id), async () =>
            {
                return await this.storageAccessService.GetTableEntityAsync<JsonTableEntity<UserEntity>>(TableNames.Users, StaticPartitionKeys.User, id);
            }, TimeSpan.FromMinutes(1));

            return result;
        }

        private string GetUserCacheKey(string userId) => $"User:{userId}";


        public async Task<CreateOrUpdateUserResult> CreateOrUpdateUserAsync(CreateOrUpdateUserCommand request)
        {
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

                savedUser.Entity.LastOnline = DateTime.UtcNow;
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

                await this.storageAccessService.ReplaceAsync(TableNames.Users, savedUser);
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
                await this.storageAccessService.InsertAsync(TableNames.Users, new JsonTableEntity<UserEntity>(StaticPartitionKeys.User, request.UserId, user));
                isNewUser = true;
            }

            return new CreateOrUpdateUserResult(isNewUser, profilePicHasChanged, nameHasChanged, originalUserName);
        }

        public async Task UpdateUserAsync(UserEntity user)
        {
            var userEntity = await FindUserEntityAsync(user.Id);
            userEntity.Entity = user;

            await this.storageAccessService.ReplaceAsync(TableNames.Users, userEntity);

            cacheService.Remove(GetUserCacheKey(user.Id));
        }

        public async Task<IEnumerable<string>> GetAllUserIdsAsync()
        {
            return await this.storageAccessService.GetRowKeysAsync(TableNames.Users, StaticPartitionKeys.User);
        }

        public async Task<List<UserEntity>> SearchUsersAsync(IEnumerable<string> userIds = null, string filterText = null)
        {
            string whereClause = BuildWhereClause(userIds);

            var result = await storageAccessService.QueryTableAsync<JsonTableEntity<UserEntity>>(TableNames.Users, whereClause);

            var users = result.OrderByDescending(u => u.Timestamp).Select(r =>
            {
                var user = r.Entity;
                user.LastOnline = r.Timestamp.UtcDateTime;
                return user;
            }).ToList();

            // TODO: Should soon be improved!
            if (!string.IsNullOrEmpty(filterText))
            {
                string lowerFilter = filterText.ToLower();
                users = users.Where(u => u.Name.ToLower().Contains(lowerFilter)).ToList();
            }

            return users;
        }

        private string BuildWhereClause(IEnumerable<string> userIds)
        {
            if (userIds == null)
            {
                return null;
            }

            string whereClause = null;

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

            return whereClause;
        }
    }
}
