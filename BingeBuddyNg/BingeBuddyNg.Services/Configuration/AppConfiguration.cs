using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Configuration
{
    public class AppConfiguration
    {
        public string StorageConnectionString { get; set; }

        public AppConfiguration(string storageConnectionString)
        {
            this.StorageConnectionString = storageConnectionString ?? throw new ArgumentNullException(nameof(storageConnectionString));
        }
    }
}
