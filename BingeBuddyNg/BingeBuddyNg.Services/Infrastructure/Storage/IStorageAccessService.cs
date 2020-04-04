using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;

namespace BingeBuddyNg.Services.Infrastructure
{
    public interface IStorageAccessService
    {
        Task AddQueueMessage(string queueName, object message);

        Task DeleteTableEntityAsync(string tableName, string partitionKey, string rowKey);

        Task<string> GetFileFromStorageAsync(string containerName, string fullPath);

        CloudQueue GetQueueReference(string queueName);

        Task<T> GetTableEntityAsync<T>(string tableName, string partitionKey, string rowKey) where T : ITableEntity, new();

        CloudTable GetTableReference(string tableName);

        Task<List<T>> QueryTableAsync<T>(string tableName, string whereClause = null) where T : ITableEntity, new();

        Task<PagedQueryResult<T>> QueryTableAsync<T>(string tableName, string whereClause, int pageSize, TableContinuationToken continuationToken = null) where T : ITableEntity, new();

        Task<string> SaveFileInBlobStorage(string containerName, string fullPath, Stream file);

        Task<string> SaveFileInBlobStorage(string containerName, string path, string fileName, Stream file);
    }
}