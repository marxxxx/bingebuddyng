using System.Collections.Generic;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Infrastructure
{
    public interface INotificationService
    {
        void SendWebPushMessage(IEnumerable<PushInfo> receivers, NotificationMessage message);

        Task SendSignalRMessageAsync(IEnumerable<string> userIds, string hubName, string method, object payload);
    }
}
