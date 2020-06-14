using System.Threading.Tasks;
using BingeBuddyNg.Services.User;

namespace BingeBuddyNg.Services.Calculation
{
    public interface ICalculationService
    {
        Task<DrinkCalculationResult> CalculateStatsForUserAsync(string userId, Gender gender, int? weight);
    }
}
