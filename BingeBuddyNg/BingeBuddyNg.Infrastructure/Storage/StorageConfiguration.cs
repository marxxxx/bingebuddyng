using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Infrastructure
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
