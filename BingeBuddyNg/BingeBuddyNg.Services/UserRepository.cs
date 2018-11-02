﻿using BingeBuddyNg.Services.Entitys;
using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Messages;
using BingeBuddyNg.Services.Models;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
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
            bool profilePicHasChanged = false;
            var savedUser = await FindUserEntityAsync(user.Id);
            
            if (savedUser != null)
            {
                profilePicHasChanged = savedUser.Entity.ProfileImageUrl != user.ProfileImageUrl;
                savedUser.Entity.Name = user.Name;
                savedUser.Entity.ProfileImageUrl = user.ProfileImageUrl;
                savedUser.Entity.PushInfo = user.PushInfo;
                saveUserOperation = TableOperation.Replace(savedUser);
            }
            else
            {
                saveUserOperation = TableOperation.Insert(new JsonTableEntity<User>(PartitionKeyValue, user.Id, user));
            }

            await table.ExecuteAsync(saveUserOperation);

            // enqueue profile image update change
            if (profilePicHasChanged)
            {
                var queue = StorageAccess.GetQueueReference("profile-update");
                var message = new ProfileUpdateMessage(user.Id, user.ProfileImageUrl);
                await queue.AddMessageAsync(new CloudQueueMessage(JsonConvert.SerializeObject(message)));
            }
        }

        public async Task UpdateUserAsync(User user)
        {
            var table = StorageAccess.GetTableReference(TableName);
            var userEntity = await FindUserEntityAsync(user.Id);
            userEntity.Entity = user;
            
            TableOperation saveUserOperation = TableOperation.Replace(userEntity);

            await table.ExecuteAsync(saveUserOperation);            
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var result = await StorageAccess.QueryTableAsync<JsonTableEntity<User>>(TableName);
            var users = result.OrderByDescending(u=>u.Timestamp).Select(r => r.Entity).ToList();
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
