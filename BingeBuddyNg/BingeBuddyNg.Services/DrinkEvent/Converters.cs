using BingeBuddyNg.Core.DrinkEvent.DTO;
using BingeBuddyNg.Core.DrinkEvent.Persistence;

namespace BingeBuddyNg.Core.DrinkEvent
{
    public static class Converters
    {
        public static DrinkEventEntity ToEntity(this Domain.DrinkEvent drinkEvent)
        {
            return new DrinkEventEntity()
            {
                StartUtc = drinkEvent.StartUtc,
                EndUtc = drinkEvent.EndUtc
            };
        }

        public static DrinkEventDTO ToDto(this Domain.DrinkEvent drinkEvent)
        {
            return new DrinkEventDTO()
            {
                StartUtc = drinkEvent.StartUtc,
                EndUtc = drinkEvent.EndUtc,
                ScoringUserIds = drinkEvent.ScoringUserIds
            };
        }
    }
}
