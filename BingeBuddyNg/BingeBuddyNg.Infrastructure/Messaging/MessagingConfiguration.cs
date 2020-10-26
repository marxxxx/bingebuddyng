using System;

namespace BingeBuddyNg.Infrastructure
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