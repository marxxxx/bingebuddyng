using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Infrastructure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace BingeBuddyNg.Infrastructure
{
    public class StorageAccessService : IStorageAccessService
    {
        private const int MaxQueryLimit = 250;
        private readonly StorageConfiguration config;
        private readonly TableRequestOptions DefaultRequestOptions = new TableRequestOptions() { RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(1), 3) };

        public StorageAccessService(StorageConfiguration config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public CloudTable GetTableReference(string tableName)
        {
            CloudStorageAccount account = GetStorageAccount();
            var client = account.CreateCloudTableClient();
            var table = client.GetTableReference(tableName);
            return table;
        }

        public async Task DeleteTableEntityAsync(string tableName, string partitionKey, string rowKey)
        {
            var table = GetTableReference(tableName);

            var retrieveOperation = TableOperation.Retrieve(partitionKey, rowKey);
            var result = await table.ExecuteAsync(retrieveOperation);
            if (result.Result != null)
            {
                var deleteOperation = TableOperation.Delete((ITableEntity)result.Result);
                await table.ExecuteAsync(deleteOperation);
            }
        }

        public async Task<T> GetTableEntityAsync<T>(string tableName, string partitionKey, string rowKey) where T : ITableEntity, new()
        {
            var table = GetTableReference(tableName);

            var operation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            var result = await table.ExecuteAsync(operation);

            return (T)result.Result;
        }

        public async Task<PagedQueryResult<T>> QueryTableAsync<T>(string tableName, string partitionKey, string minRowKey, int pageSize, TableContinuationToken continuationToken = null) where T : ITableEntity, new()
        {
            string whereClause = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey);

            if (string.IsNullOrEmpty(minRowKey) == false)
            {
                whereClause = TableQuery.CombineFilters(whereClause, TableOperators.And,
                        TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, minRowKey));
            }

            return await QueryTableAsync<T>(tableName, whereClause, pageSize, continuationToken);
        }

        public async Task<List<T>> QueryTableAsync<T>(string tableName, string whereClause = null) where T : ITableEntity, new()
        {
            List<T> resultList = new List<T>();
            TableContinuationToken continuationToken = null;

            do
            {
                var result = await QueryTableAsync<T>(tableName, whereClause, MaxQueryLimit, continuationToken);

                resultList.AddRange(result.ResultPage);
                continuationToken = result.ContinuationToken?.ToContinuationToken();
            } while (continuationToken != null);

            return resultList;
        }

        public async Task<PagedQueryResult<T>> QueryTableAsync<T>(string tableName, string whereClause, int pageSize,
            TableContinuationToken continuationToken = null) where T : ITableEntity, new()
        {
            var table = GetTableReference(tableName);

            TableQuery<T> tableQuery = new TableQuery<T>();
            if (whereClause != null)
            {
                tableQuery = tableQuery.Where(whereClause);
            }

            tableQuery = tableQuery.Take(pageSize);

            // Retrieve a segment
            TableQuerySegment<T> tableQueryResult =
                await table.ExecuteQuerySegmentedAsync<T>(tableQuery, continuationToken);

            var result = tableQueryResult.Results;
            return new PagedQueryResult<T>(result, tableQueryResult.ContinuationToken);
        }

        public async Task<IEnumerable<string>> GetRowKeysAsync(string tableName, string partitionKey)
        {
            var query = new TableQuery<DynamicTableEntity>()
            {
                FilterString = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey),
                SelectColumns = new List<string>()
                {
                    "RowKey"
                }
            };

            List<string> rowKeys = new List<string>();

            var table = this.GetTableReference(tableName);
            TableContinuationToken continuationToken = null;
            do
            {
                var tableResult = await table.ExecuteQuerySegmentedAsync(query, continuationToken);
                rowKeys.AddRange(tableResult.Select(t => t.RowKey).ToList());
                continuationToken = tableResult.ContinuationToken;
            } while (continuationToken != null);

            return rowKeys;
        }

        public CloudQueue GetQueueReference(string queueName)
        {
            CloudStorageAccount account = GetStorageAccount();
            var client = account.CreateCloudQueueClient();
            var queue = client.GetQueueReference(queueName);
            return queue;
        }

        public Task AddQueueMessage(string queueName, object message)
        {
            var queue = GetQueueReference(queueName);
            return queue.AddMessageAsync(new CloudQueueMessage(JsonConvert.SerializeObject(message)));
        }

        public async Task<string> GetFileFromStorageAsync(string containerName, string fullPath)
        {
            var account = GetStorageAccount();
            var blobClient = account.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fullPath);

            var content = await blockBlob.DownloadTextAsync();
            return content;
        }

        public async Task<string> SaveFileInBlobStorage(string containerName, string fullPath, Stream file)
        {
            var account = GetStorageAccount();
            var blobClient = account.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference(containerName);

            var blob = container.GetBlockBlobReference(fullPath);
            await blob.UploadFromStreamAsync(file);

            var fileUrl = blob.Uri.AbsoluteUri;

            return fileUrl;
        }

        public Task<string> SaveFileInBlobStorage(string containerName, string path, string fileName, Stream file)
        {
            string fullPath = $"{path}/{Guid.NewGuid()}_{fileName}";

            return SaveFileInBlobStorage(containerName, fullPath, file);
        }

        private CloudStorageAccount GetStorageAccount()
        {
            return CloudStorageAccount.Parse(this.config.StorageConnectionString);
        }

        public async Task InsertAsync(string tableName, ITableEntity entity)
        {
            var table = this.GetTableReference(tableName);

            TableOperation operation = TableOperation.Insert(entity);
            await table.ExecuteAsync(operation, DefaultRequestOptions, null);
        }

        public async Task ReplaceAsync(string tableName, ITableEntity entity)
        {
            var table = this.GetTableReference(tableName);

            TableOperation operation = TableOperation.Replace(entity);
            await table.ExecuteAsync(operation, DefaultRequestOptions, null);
        }

        public async Task DeleteAsync(string tableName, ITableEntity entity)
        {
            var table = this.GetTableReference(tableName);

            TableOperation operation = TableOperation.Delete(entity);
            await table.ExecuteAsync(operation, DefaultRequestOptions, null);
        }

        public async Task DeleteAsync(string tableName, string partitionKey, string rowKey)
        {
            var entity = await GetTableEntityAsync<DynamicTableEntity>(tableName, partitionKey, rowKey);
            if (entity != null)
            {
                await DeleteAsync(tableName, entity);
            }
        }

        public async Task InsertOrReplaceAsync(string tableName, IEnumerable<ITableEntity> entities)
        {
            var table = GetTableReference(tableName);

            var partitionedBatches = entities.GroupBy(e => e.PartitionKey);
            foreach (var partitionedBatch in partitionedBatches)
            {
                TableBatchOperation batch = new TableBatchOperation();

                foreach (var e in partitionedBatch)
                {
                    batch.Add(TableOperation.InsertOrReplace(e));
                }

                await table.ExecuteBatchAsync(batch, DefaultRequestOptions, null);
            }
        }

        public async Task InsertOrReplaceAsync(string tableName, ITableEntity entity)
        {
            var table = this.GetTableReference(tableName);

            TableOperation operation = TableOperation.InsertOrReplace(entity);
            await table.ExecuteAsync(operation, DefaultRequestOptions, null);
        }

        public async Task InsertOrMergeAsync(string tableName, ITableEntity entity)
        {
            var table = this.GetTableReference(tableName);

            TableOperation operation = TableOperation.InsertOrMerge(entity);
            await table.ExecuteAsync(operation, DefaultRequestOptions, null);
        }
    }
}
