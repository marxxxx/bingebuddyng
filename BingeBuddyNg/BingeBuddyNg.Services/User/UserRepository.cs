using System;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Core.User.Commands;
using BingeBuddyNg.Core.User.Domain;
using BingeBuddyNg.Core.User.Persistence;
using BingeBuddyNg.Core.Venue;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Core.User
{
    public class UserRepository : IUserRepository
    {
        private readonly IStorageAccessService storageAccess;
        private readonly ICacheService cacheService;

        public UserRepository(IStorageAccessService storageAccess, ICacheService cacheService)
        {
            this.storageAccess = storageAccess ?? throw new ArgumentNullException(nameof(storageAccess));
            this.cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        public async Task<Core.User.Domain.User> GetUserAsync(string id)
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

            return new Core.User.Domain.User(user.Id, user.Name, user.Weight, user.Gender, user.ProfileImageUrl, user.PushInfo, user.Friends, user.MutedByFriendUserIds, user.MutedByFriendUserIds, user.MonitoringInstanceId, user.CurrentVenue?.ToDomain(), user.Language, user.LastOnline);
        }

        private async Task<JsonTableEntity<UserEntity>> FindUserEntityAsync(string id)
        {
            var result = await cacheService.GetOrCreateAsync(GetUserCacheKey(id), async () =>
            {
                return await this.storageAccess.GetTableEntityAsync<JsonTableEntity<UserEntity>>(TableNames.Users, StaticPartitionKeys.User, id);
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

                await this.storageAccess.ReplaceAsync(TableNames.Users, savedUser);
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
                await this.storageAccess.InsertAsync(TableNames.Users, new JsonTableEntity<UserEntity>(StaticPartitionKeys.User, request.UserId, user));
                isNewUser = true;
            }

            return new CreateOrUpdateUserResult(isNewUser, profilePicHasChanged, nameHasChanged, originalUserName);
        }

        public async Task UpdateUserAsync(UserEntity user)
        {
            var userEntity = await FindUserEntityAsync(user.Id);
            userEntity.Entity = user;

            await this.storageAccess.ReplaceAsync(TableNames.Users, userEntity);

            cacheService.Remove(GetUserCacheKey(user.Id));
        }
    }
}
