using Microsoft.WindowsAzure.Storage.Table;

namespace BingeBuddyNg.Core.User.Persistence
{
    public class MonitoringTableEntity : TableEntity
    {
        public string UserId { get; set; }

        public string MonitoringInstanceId { get; set; }

        public MonitoringTableEntity()
        {
        }

        public MonitoringTableEntity(string userId, string monitoringInstanceId) : base(userId, userId)
        {
            this.UserId = userId;
            this.MonitoringInstanceId = monitoringInstanceId;
        }
    }
}