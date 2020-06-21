using System;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Venue;
using BingeBuddyNg.Core.Infrastructure;
using Microsoft.Extensions.Logging;

namespace BingeBuddyNg.Services.Venue
{
    public class VenueUserRepository : IVenueUserRepository
    {
        private const string TableName = "venueusers";

        private readonly IStorageAccessService storageAccessService;
        private readonly ILogger<VenueUserRepository> logger;

        public VenueUserRepository(IStorageAccessService storageAccessService, ILogger<VenueUserRepository> logger)
        {
            this.storageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task AddUserToVenueAsync(string venueId, string venueName, string userId, string userName)
        {
            var entity = new VenueUsersTableEntity(venueId, venueName, userId, userName);

            await this.storageAccessService.InsertAsync(TableName, entity);
        }

        public async Task RemoveUserFromVenueAsync(string venueId, string userId)
        {
            await this.storageAccessService.DeleteAsync(TableName, venueId, userId);
        }
    }
}
