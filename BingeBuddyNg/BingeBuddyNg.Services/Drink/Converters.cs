using BingeBuddyNg.Core.Drink.DTO;
using BingeBuddyNg.Core.Drink.Persistence;

namespace BingeBuddyNg.Core.Drink
{
    internal static class Converters
    {
        internal static DrinkDTO ToDto(this DrinkTableEntity entity)
        {
            return new DrinkDTO()
            {
                Id = entity.Id,
                DrinkType = entity.DrinkType
            };
        }

        internal static DrinkTableEntity ToEntity(this DrinkDTO drink, string userId)
        {
            return new DrinkTableEntity(userId, drink);
        }
    }
}
