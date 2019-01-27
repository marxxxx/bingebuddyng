using System;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;

namespace BingeBuddyNg.Services.Venue
{
    public class VenueUserRepository : IVenueUserRepository
    {
        private const string TableName = "venueusers";

        public StorageAccessService StorageAccessService { get; set; }
        private ILogger<VenueUserRepository> logger;

        public VenueUserRepository(StorageAccessService storageAccessService, ILogger<VenueUserRepository> logger)
        {
            StorageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task AddUserToVenueAsync(string venueId, string venueName, string userId, string userName)
        {
            var table = StorageAccessService.GetTableReference(TableName);
            var entity = new VenueUsersTableEntity(venueId, venueName, userId, userName);

            await table.ExecuteAsync(TableOperation.Insert(entity));
        }

        public async Task RemoveUserFromVenueAsync(string venueId, string userId)
        {
            var table = StorageAccessService.GetTableReference(TableName);

            var entity = await StorageAccessService.GetTableEntityAsync<VenueUsersTableEntity>(TableName, venueId, userId);

            await table.ExecuteAsync(TableOperation.Delete(entity));
        }
    }
}
