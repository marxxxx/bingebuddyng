using BingeBuddyNg.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Interfaces
{
    public interface INotificationService
    {
        void SendMessage(IEnumerable<PushInfo> receivers, NotificationMessage message);
    }
}
