using System;

namespace BingeBuddyNg.Core.Activity.Messages
{
    public class DrinkEventMessage
    {
        public string UserId { get; set; }

        public string DrinkId { get; set; }

        public DateTime Timestamp { get; set; }

        public DrinkEventMessage(string userId, string drinkId, DateTime timestamp)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            DrinkId = drinkId ?? throw new ArgumentNullException(nameof(drinkId));
            Timestamp = timestamp;
        }

        public override string ToString()
        {
            return $"{{{nameof(UserId)}={UserId}, {nameof(DrinkId)}={DrinkId}, {nameof(Timestamp)}={Timestamp.ToString()}}}";
        }
    }
}
