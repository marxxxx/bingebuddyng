using System.Collections.Generic;
using System.Threading.Tasks;

namespace BingeBuddyNg.Core.Infrastructure
{
    public interface INotificationService
    {
        void SendWebPushMessage(IEnumerable<PushInfo> receivers, WebPushNotificationMessage message);

        Task SendSignalRMessageAsync(IEnumerable<string> userIds, string hubName, string method, object payload);
    }
}
