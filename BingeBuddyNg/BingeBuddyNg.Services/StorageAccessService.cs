using BingeBuddyNg.Services.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
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

        public async Task<List<T>> QueryTableAsync<T>(string tableName, string whereClause) where T : ITableEntity, new()
        {
            // Initialize the continuation token to null to start from the beginning of the table.
            TableContinuationToken continuationToken = null;

            var table = GetTableReference(tableName);

            TableQuery<T> tableQuery = new TableQuery<T>().Where(whereClause);

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


        private CloudStorageAccount GetStorageAccount()
        {
            return CloudStorageAccount.Parse(this.config.StorageConnectionString);
        }

    }
}
