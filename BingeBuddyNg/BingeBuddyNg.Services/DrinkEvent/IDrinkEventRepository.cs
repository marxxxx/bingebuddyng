using System;
using System.Threading.Tasks;

namespace BingeBuddyNg.Core.DrinkEvent
{
    public interface IDrinkEventRepository
    {
        Task<Domain.DrinkEvent> FindCurrentDrinkEventAsync();

        Task<Domain.DrinkEvent> CreateDrinkEventAsync(DateTime startTime, DateTime endTime);

        Task UpdateDrinkEventAsync(Domain.DrinkEvent drinkEvent);
    }
}
