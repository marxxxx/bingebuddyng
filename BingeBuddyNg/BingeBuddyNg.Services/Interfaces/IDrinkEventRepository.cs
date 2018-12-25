using BingeBuddyNg.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Interfaces
{
    public interface IDrinkEventRepository
    {
        Task<DrinkEvent> FindCurrentDrinkEventAsync();
        Task<DrinkEvent> CreateDrinkEventAsync(DateTime startTime, DateTime endTime);
        Task UpdateDrinkEventAsync(DrinkEvent drinkEvent);
    }
}
