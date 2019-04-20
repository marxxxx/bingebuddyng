﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Drink
{
    public interface IDrinkRepository
    {
        Task DeleteDrinkAsync(string userId, string drinkId);
        Task<IEnumerable<Drink>> GetDrinksAsync(string userId);
        Task SaveDrinkAsync(string userId, Drink drink);
    }
}