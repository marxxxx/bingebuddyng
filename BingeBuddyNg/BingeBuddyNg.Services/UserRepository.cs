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
    public class UserRepository : IUserRepository
    {
        private const string TableName = "users";
        private const string PartitionKeyValue = "User";

        public StorageAccessService StorageAccess { get; }

        public UserRepository(StorageAccessService storageAccess)
        {
            this.StorageAccess = storageAccess ?? throw new ArgumentNullException(nameof(storageAccess));
        }
                

        public async Task<User> FindUserAsync(string id)
        {
            var result = await FindUserEntityAsync(id);
            User user = null;
            if (result != null)
            {
                user = result.Entity;
            }

            return user;
        }

        private async Task<JsonTableEntity<User>> FindUserEntityAsync(string id)
        {
            var table = StorageAccess.GetTableReference(TableName);

            TableOperation retrieveOperation = TableOperation.Retrieve<JsonTableEntity<User>>(PartitionKeyValue, id);

            var result = await table.ExecuteAsync(retrieveOperation);

            return result?.Result as JsonTableEntity<User>;
        }


        public async Task CreateOrUpdateUserAsync(User user)
        {
            var table = StorageAccess.GetTableReference(TableName);
            TableOperation saveUserOperation = null;
            var savedUser = await FindUserEntityAsync(user.Id);
            if(savedUser != null)
            {
                savedUser.Entity.Name = user.Name;
                savedUser.Entity.ProfileImageUrl = user.ProfileImageUrl;
                savedUser.Entity.PushInfo = user.PushInfo;
                saveUserOperation = TableOperation.Replace(savedUser);
            }
            else
            {
                saveUserOperation = TableOperation.Insert(new JsonTableEntity<User>(PartitionKeyValue, user.Id, savedUser.Entity));
            }

            await table.ExecuteAsync(saveUserOperation);
        }

        public Task UpdateUserAsync(User user)
        {
            var table = StorageAccess.GetTableReference(TableName);
            
            TableOperation saveUserOperation = TableOperation.Replace(new JsonTableEntity<User>(PartitionKeyValue, user.Id, user));

            return table.ExecuteAsync(saveUserOperation);            
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var result = await StorageAccess.QueryTableAsync<JsonTableEntity<User>>(TableName);
            var users = result.Select(r => r.Entity).ToList();
            return users;
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
    }
}
