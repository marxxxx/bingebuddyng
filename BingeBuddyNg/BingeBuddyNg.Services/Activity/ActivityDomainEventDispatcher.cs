using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Activity.Domain.Events;
using BingeBuddyNg.Services.Infrastructure.EventGrid;
using System;
using BingeBuddyNg.Services.Infrastructure;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Services.Activity
{
    public class ActivityDomainEventDispatcher
    {
        private readonly IEventGridService eventGridService;
        private readonly IStorageAccessService storageAccessService;

        public ActivityDomainEventDispatcher(IEventGridService eventGridService, IStorageAccessService storageAccessService)
        {
            this.eventGridService = eventGridService ?? throw new ArgumentNullException(nameof(eventGridService));
            this.storageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
        }

        public async Task Dispatch(IEnumerable<IDomainEvent> events)
        {
            foreach (var e in events)
            {
                switch (e)
                {
                    case ActivityAdded a:
                        {
                            await this.eventGridService.PublishAsync("ActivityAdded", new ActivityAddedMessage(a.Id));
                            break;
                        }
                    case ReactionAdded r:
                        {
                            await this.storageAccessService.AddQueueMessage(QueueNames.ReactionAdded, new ReactionAddedMessage(r.ActivityId, r.ReactionType, r.UserId, r.Comment));
                            break;
                        }
                }
            }

        }
    }
}
