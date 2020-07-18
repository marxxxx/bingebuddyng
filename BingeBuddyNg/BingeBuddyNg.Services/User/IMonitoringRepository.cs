using BingeBuddyNg.Core.User.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BingeBuddyNg.Core.User
{
    public interface IMonitoringRepository
    {
        Task<MonitoringTableEntity> FindAsync(string userId);

        Task SaveAsync(string userId, string monitoringInstanceId);

        Task DeleteAsync(string userId);
    }
}
