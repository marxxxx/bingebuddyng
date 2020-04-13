using System;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.DrinkEvent
{
    public interface IDrinkEventRepository
    {
        Task<DrinkEvent> FindCurrentDrinkEventAsync();

        Task<DrinkEvent> CreateDrinkEventAsync(DateTime startTime, DateTime endTime);

        Task UpdateDrinkEventAsync(DrinkEvent drinkEvent);
    }
}
