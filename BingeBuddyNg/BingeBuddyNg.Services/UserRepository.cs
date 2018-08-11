using BingeBuddyNg.Services.Entitys;
using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Models;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services
{
    public class UserRepository : IUserRepository
    {
        private const string TableName = "users";

        public StorageAccessService StorageAccess { get; }

        public UserRepository(StorageAccessService storageAccess)
        {
            this.StorageAccess = storageAccess ?? throw new ArgumentNullException(nameof(storageAccess));
        }
                

        public async Task<User> GetUserAsync(string id)
        {
            var table = StorageAccess.GetTableReference(TableName);

            TableOperation retrieveOperation = TableOperation.Retrieve<UserTableEntity>(UserTableEntity.PartitionKeyValue, id);
            
            var result = await table.ExecuteAsync(retrieveOperation);

            var userEntity = (UserTableEntity)result.Result;

            var model = EntityConverters.Users.EntityToModel(userEntity);
            return model;
        }

        public Task SaveUserAsync(User user)
        {
            var table = StorageAccess.GetTableReference(TableName);

            UserTableEntity userEntity = EntityConverters.Users.ModelToEntity(user);

            TableOperation saveUserOperation = TableOperation.InsertOrReplace(userEntity);

            return table.ExecuteAsync(saveUserOperation);            
        }
    }
}
