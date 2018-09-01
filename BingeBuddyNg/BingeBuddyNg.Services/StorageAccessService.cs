using BingeBuddyNg.Services.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services
{
    public class StorageAccessService
    {
        private AppConfiguration config;

        public StorageAccessService(AppConfiguration config)
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

        public async Task<T> GetTableEntityAsync<T>(string tableName, string partitionKey, string rowKey) where T:ITableEntity, new()
        {
            var table = GetTableReference(tableName);

            var operation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            var result = await table.ExecuteAsync(operation);

            return (T)result.Result;
        }

        public async Task<List<T>> QueryTableAsync<T>(string tableName, string whereClause=null) where T : ITableEntity, new()
        {
            // Initialize the continuation token to null to start from the beginning of the table.
            TableContinuationToken continuationToken = null;

            var table = GetTableReference(tableName);

            TableQuery<T> tableQuery = new TableQuery<T>();
            if(whereClause != null)
            {
                tableQuery = tableQuery.Where(whereClause);
            }

            // Retrieve a segment (up to 1,000 entities) -> we never want more at once!
            TableQuerySegment<T> tableQueryResult =
                await table.ExecuteQuerySegmentedAsync<T>(tableQuery, continuationToken);

            var result = tableQueryResult.Results;
            return result;
        }


        public CloudQueue GetQueueReference(string queueName)
        {
            CloudStorageAccount account = GetStorageAccount();
            var client = account.CreateCloudQueueClient();
            var queue = client.GetQueueReference(queueName);
            return queue;
        }

        public async Task<string> SaveFileInBlobStorage(string containerName, string path, string fileName, Stream file)
        {
            var account = GetStorageAccount();
            var blobClient =  account.CreateCloudBlobClient();
            
            var container = blobClient.GetContainerReference(containerName);

            string fullPath = $"{path}/{Guid.NewGuid()}_{fileName}";
            var blob = container.GetBlockBlobReference(fullPath);
            await blob.UploadFromStreamAsync(file);

            var fileUrl = blob.Uri.AbsoluteUri;

            return fileUrl;
        }

        private CloudStorageAccount GetStorageAccount()
        {
            return CloudStorageAccount.Parse(this.config.StorageConnectionString);
        }

    }
}
