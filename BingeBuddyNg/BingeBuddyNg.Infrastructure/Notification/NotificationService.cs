using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.SignalR.Management;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WebPush;

namespace BingeBuddyNg.Services.Infrastructure
{
    public class NotificationService : INotificationService
    {
        private readonly WebPushConfiguration configuration;
        private readonly IServiceManager serviceManager;
        private readonly ILogger<NotificationService> logger;

        public NotificationService(WebPushConfiguration configuration, IServiceManager serviceManager, ILogger<NotificationService> logger)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.serviceManager = serviceManager ?? throw new ArgumentNullException(nameof(serviceManager));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void SendWebPushMessage(IEnumerable<PushInfo> receivers, WebPushNotificationMessage message)
        {
            var webPushClient = new WebPushClient();
            var vapidDetails = new VapidDetails("mailto:brewmaster@bingebuddyng.com",
                configuration.WebPushPublicKey, configuration.WebPushPrivateKey);

            var pushMessage = new WebPushMessage(message);

            foreach (var pushInfo in receivers)
            {
                try
                {
                    webPushClient.SendNotification(
                        new PushSubscription(pushInfo.SubscriptionEndpoint, pushInfo.p256dh, pushInfo.Auth),
                        JsonConvert.SerializeObject(pushMessage),
                        vapidDetails);
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, $"Error sending push notification!");
                }
            }
        }

        public async Task SendSignalRMessageAsync(IEnumerable<string> userIds, string hubName, string method, object payload)
        {
            try
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

                await hubContext.Clients.Users(new List<string>(userIds).AsReadOnly()).SendCoreAsync(method, new[] { serialized });
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error sending SignalR notification!");
            }
        }
    }
}
