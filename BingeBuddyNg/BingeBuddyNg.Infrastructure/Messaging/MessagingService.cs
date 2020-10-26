using System;
using System.Text;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Infrastructure;
using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;

namespace BingeBuddyNg.Infrastructure
{
    public class MessagingService : IMessagingService
    {
        private const string EventHubName = "bingehub";

        private readonly MessagingConfiguration config;

        public MessagingService(MessagingConfiguration config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task SendMessageAsync<T>(T message)
        {
            var client = CreateClient();

            await client.SendAsync(new EventData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message))));

            await client.CloseAsync();
        }

        private EventHubClient CreateClient()
        {
            // Creates an EventHubsConnectionStringBuilder object from the connection string, and sets the EntityPath.
            // Typically, the connection string should have the entity path in it, but this simple scenario
            // uses the connection string from the namespace.
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(config.EventHubConnectionString)
            {
                EntityPath = EventHubName
            };

            var eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            return eventHubClient;
        }
    }
}