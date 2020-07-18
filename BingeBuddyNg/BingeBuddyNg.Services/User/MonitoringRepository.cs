using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Core.User.Persistence;
using System;
using System.Threading.Tasks;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Core.User
{
    public class MonitoringRepository : IMonitoringRepository
    {
        private readonly IStorageAccessService storageAccessService;

        public MonitoringRepository(IStorageAccessService storageAccessService)
        {
            this.storageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
        }

        public async Task<MonitoringTableEntity> FindAsync(string userId)
        {
            return await this.storageAccessService.GetTableEntityAsync<MonitoringTableEntity>(TableNames.Monitoring, userId, userId);
        }

        public async Task SaveAsync(string userId, string monitoringInstanceId)
        {
            var entity = new MonitoringTableEntity(userId, monitoringInstanceId);
            await this.storageAccessService.InsertOrReplaceAsync(TableNames.Monitoring, entity);
        }

        public async Task DeleteAsync(string userId)
        {
            await this.storageAccessService.DeleteAsync(TableNames.Monitoring, userId, userId);
        }
    }
}
