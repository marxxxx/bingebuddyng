using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using WebPush;

namespace BingeBuddyNg.Services.Infrastructure
{
    public class NotificationService : INotificationService
    {
        private AppConfiguration configuration;

        public NotificationService(AppConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void SendMessage(IEnumerable<PushInfo> receivers, NotificationMessage message)
        {
            var webPushClient = new WebPushClient();
            var vapidDetails = new VapidDetails("mailto:brewmaster@bingebuddyng.com", 
                configuration.WebPushPublicKey, configuration.WebPushPrivateKey);

            var pushMessage = new WebPushMessage(message);            

            foreach (var pushInfo in receivers)
            {
                webPushClient.SendNotification(new PushSubscription(pushInfo.SubscriptionEndpoint, pushInfo.p256dh, pushInfo.Auth), 
                    JsonConvert.SerializeObject(pushMessage), vapidDetails);
            }
        }
    }
}
