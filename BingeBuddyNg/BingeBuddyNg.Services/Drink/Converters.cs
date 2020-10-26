using BingeBuddyNg.Core.Drink.DTO;
using BingeBuddyNg.Core.Drink.Persistence;

namespace BingeBuddyNg.Core.Drink
{
    public static class Converters
    {
        public static DrinkDTO ToDto(this DrinkTableEntity entity)
        {
            return new DrinkDTO(id: entity.Id, drinkType: entity.DrinkType, name: entity.Name, alcPrc: entity.AlcPrc, volume: entity.Volume);
        }

        public static DrinkTableEntity ToEntity(this DrinkDTO drink, string userId)
        {
            return new DrinkTableEntity(userId, drink);
        }
    }
}