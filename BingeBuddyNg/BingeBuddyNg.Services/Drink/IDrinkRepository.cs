using System.Collections.Generic;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Drink.DTO;

namespace BingeBuddyNg.Core.Drink
{
    public interface IDrinkRepository
    {
        Task<IEnumerable<DrinkDTO>> GetDrinksAsync(string userId);

        Task<DrinkDTO> GetDrinkAsync(string userId, string drinkId);

        Task CreateDefaultDrinksForUserAsync(string userId);

        Task SaveDrinksAsync(string userId, IEnumerable<DrinkDTO> drinks);

        Task DeleteDrinkAsync(string userId, string drinkId);
    }
}