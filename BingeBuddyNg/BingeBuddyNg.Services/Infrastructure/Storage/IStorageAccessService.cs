using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace BingeBuddyNg.Services.Infrastructure
{
    public interface IStorageAccessService
    {
        // Queues
        Task AddQueueMessage(string queueName, object message);

        // Blob
        Task<string> GetFileFromStorageAsync(string containerName, string fullPath);

        Task<string> SaveFileInBlobStorage(string containerName, string fullPath, Stream file);

        Task<string> SaveFileInBlobStorage(string containerName, string path, string fileName, Stream file);

        // Tables
        Task<T> GetTableEntityAsync<T>(string tableName, string partitionKey, string rowKey) where T : ITableEntity, new();

        Task<PagedQueryResult<T>> QueryTableAsync<T>(string tableName, string partitionKey, string minRowKey = null, int pageSize = 100, TableContinuationToken continuationToken = null) where T : ITableEntity, new();

        Task<List<T>> QueryTableAsync<T>(string tableName, string whereClause = null) where T : ITableEntity, new();

        Task<PagedQueryResult<T>> QueryTableAsync<T>(string tableName, string whereClause = null, int pageSize = 100, TableContinuationToken continuationToken = null) where T : ITableEntity, new();

        Task InsertOrReplaceAsync(string tableName, IEnumerable<ITableEntity> entities);

        Task InsertOrReplaceAsync(string tableName, ITableEntity entity);

        Task InsertOrMergeAsync(string tableName, ITableEntity entity);

        Task InsertAsync(string tableName, ITableEntity entity);

        Task ReplaceAsync(string tableName, ITableEntity entity);

        Task DeleteAsync(string tableName, ITableEntity entity);

        Task DeleteAsync(string tableName, string partitionKey, string rowKey);

        Task<IEnumerable<string>> GetRowKeysAsync(string tableName, string partitionKey);
    }
}