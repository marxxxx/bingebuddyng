using System;

namespace BingeBuddyNg.Infrastructure
{
    public class StorageConfiguration
    {
        public StorageConfiguration(string storageConnectionString)
        {
            StorageConnectionString = storageConnectionString ?? throw new ArgumentNullException(nameof(storageConnectionString));
        }

        public string StorageConnectionString { get; }
    }
}