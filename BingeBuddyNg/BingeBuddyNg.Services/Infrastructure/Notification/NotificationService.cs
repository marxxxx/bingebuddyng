using Microsoft.Azure.SignalR.Management;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebPush;

namespace BingeBuddyNg.Services.Infrastructure
{
    public class NotificationService : INotificationService
    {
        private readonly WebPushConfiguration configuration;
        private readonly IServiceManager serviceManager;

        public NotificationService(WebPushConfiguration configuration, IServiceManager serviceManager)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.serviceManager = serviceManager ?? throw new ArgumentNullException(nameof(serviceManager));
        }

        public void SendWebPushMessage(IEnumerable<PushInfo> receivers, NotificationMessage message)
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

        public async Task SendSignalRMessageAsync(IReadOnlyList<string> userIds, string hubName, string method, object payload)
        {
            var hubContext = await serviceManager.CreateHubContextAsync(hubName);

            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var settings = new JsonSerializerSettings
            {
                ContractResolver = contractResolver
            };

            var serialized = JsonConvert.SerializeObject(payload, settings);

            await hubContext.Clients.Users(userIds).SendCoreAsync(method, new[] { serialized });
        }
    }
}
