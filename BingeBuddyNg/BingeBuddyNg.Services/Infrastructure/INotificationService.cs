using System.Collections.Generic;

namespace BingeBuddyNg.Services.Infrastructure
{
    public interface INotificationService
    {
        void SendMessage(IEnumerable<PushInfo> receivers, NotificationMessage message);
    }
}
