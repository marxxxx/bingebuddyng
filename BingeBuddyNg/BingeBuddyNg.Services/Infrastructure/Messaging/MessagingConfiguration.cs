using System;

namespace BingeBuddyNg.Services.Infrastructure.Messaging
{
    public class MessagingConfiguration
    {
        public MessagingConfiguration(string eventHubConnectionString)
        {
            EventHubConnectionString = eventHubConnectionString ?? throw new ArgumentNullException(nameof(eventHubConnectionString));
        }

        public string EventHubConnectionString { get; set; }
    }
}
