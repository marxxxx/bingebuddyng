using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Infrastructure;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;

namespace BingeBuddyNg.Infrastructure
{
    public class EventGridService : IEventGridService
    {
        private readonly EventGridConfiguration eventGridConfiguration;

        public EventGridService(EventGridConfiguration eventGridConfiguration)
        {
            this.eventGridConfiguration = eventGridConfiguration ?? throw new ArgumentNullException(nameof(eventGridConfiguration));
        }

        public async Task PublishAsync(string type, object eventData)
        {
            string topicHostname = new Uri(this.eventGridConfiguration.Endpoint).Host;
            var topicCredentials = new TopicCredentials(this.eventGridConfiguration.TopicKey);
            var client = new EventGridClient(topicCredentials);
            await client.PublishEventsAsync(topicHostname, new List<EventGridEvent>()
            {
                new EventGridEvent(Guid.NewGuid().ToString(), type, eventData, type, DateTime.UtcNow, "1")
            });
        }
    }
}