using BingeBuddyNg.Services.Calculation;
using BingeBuddyNg.Services.Models;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Interfaces
{
    public interface ICalculationService
    {
        Task<DrinkCalculationResult> CalculateStatsForUserAsync(User user);
    }
}
