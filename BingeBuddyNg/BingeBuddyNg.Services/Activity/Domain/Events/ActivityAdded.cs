using System;

namespace BingeBuddyNg.Services.Activity.Domain.Events
{
    public class ActivityAdded : IDomainEvent
    {
        public ActivityAdded(string id)
        {
            this.Id = id ?? throw new ArgumentNullException(nameof(id));
        }

        public string Id { get; private set; }
    }
}
