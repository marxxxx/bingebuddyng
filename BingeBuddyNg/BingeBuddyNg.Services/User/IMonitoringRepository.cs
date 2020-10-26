using System.Threading.Tasks;
using BingeBuddyNg.Core.User.Persistence;

namespace BingeBuddyNg.Core.User
{
    public interface IMonitoringRepository
    {
        Task<MonitoringTableEntity> FindAsync(string userId);

        Task SaveAsync(string userId, string monitoringInstanceId);

        Task DeleteAsync(string userId);
    }
}