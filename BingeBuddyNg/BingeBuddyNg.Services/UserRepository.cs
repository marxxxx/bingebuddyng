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
                

        public async Task<User> GetUserAsync(string id)
        {
            var table = StorageAccess.GetTableReference(TableName);

            TableOperation retrieveOperation = TableOperation.Retrieve<JsonTableEntity<User>>(PartitionKeyValue, id);

            var result = await table.ExecuteAsync(retrieveOperation);

            var user = ((JsonTableEntity<User>)result.Result).Entity;
            return user;
        }

        public Task SaveUserAsync(User user)
        {
            var table = StorageAccess.GetTableReference(TableName);

            TableOperation saveUserOperation = TableOperation.InsertOrReplace(new JsonTableEntity<User>(PartitionKeyValue, user.Id, user));

            return table.ExecuteAsync(saveUserOperation);            
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var result = await StorageAccess.QueryTableAsync<JsonTableEntity<User>>(TableName);
            var users = result.Select(r => r.Entity).ToList();
            return users;
        }
    }
}
