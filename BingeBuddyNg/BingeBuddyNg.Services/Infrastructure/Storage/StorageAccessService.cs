using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace BingeBuddyNg.Services.Infrastructure
{
    public class StorageAccessService : IStorageAccessService
    {
        private StorageConfiguration config;

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
            if(result.Result != null)
            {
                var deleteOperation = TableOperation.Delete((ITableEntity)result.Result);
                await table.ExecuteAsync(deleteOperation);
            }
        }

        public async Task<T> GetTableEntityAsync<T>(string tableName, string partitionKey, string rowKey) where T:ITableEntity, new()
        {
            var table = GetTableReference(tableName);

            var operation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            var result = await table.ExecuteAsync(operation);

            return (T)result.Result;
        }

        public async Task<List<T>> QueryTableAsync<T>(string tableName, string whereClause = null) where T : ITableEntity, new()
        {
            List<T> resultList = new List<T>();
            TableContinuationToken continuationToken = null;

            do
            {
                var result = await QueryTableAsync<T>(tableName, whereClause, 250, continuationToken);

                resultList.AddRange(result.ResultPage);
                continuationToken = result.ContinuationToken?.ToContinuationToken();
            } while (continuationToken != null);

            return resultList;
        }

        public async Task<PagedQueryResult<T>> QueryTableAsync<T>(string tableName, string whereClause, int pageSize, 
            TableContinuationToken continuationToken=null) where T : ITableEntity, new()
        {
            var table = GetTableReference(tableName);

            TableQuery<T> tableQuery = new TableQuery<T>();
            if(whereClause != null)
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

            var content =  await blockBlob.DownloadTextAsync();
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

    }
}
