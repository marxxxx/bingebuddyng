using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebPush;

namespace BingeBuddyNg.Services
{
    public class NotificationService : INotificationService
    {
        private const string PushPublicKey = "BP7M6mvrmwidRr7II8ewUIRSg8n7_mKAlWagRziRRluXnMc_d_rPUoVWGHb79YexnD0olGIFe_xackYqe1fmoxo";
        private const string PushPrivateKey = "1NKizDYbqdvxaN_su5xvcC3GipJz65hD3UOmYGDFrRw";


        public void SendMessage(IEnumerable<PushInfo> receivers, NotificationMessage message)
        {
            var webPushClient = new WebPushClient();
            var vapidDetails = new VapidDetails("mailto:brewmaster@bingebuddyng.com", PushPublicKey, PushPrivateKey);

            var pushMessage = new WebPushMessage(message);            

            foreach (var pushInfo in receivers)
            {
                webPushClient.SendNotification(new PushSubscription(pushInfo.SubscriptionEndpoint, pushInfo.p256dh, pushInfo.Auth), 
                    JsonConvert.SerializeObject(pushMessage), vapidDetails);
            }


        }
    }
}
