using System.Threading.Tasks;
using BingeBuddyNg.Core.User;

namespace BingeBuddyNg.Core.Calculation
{
    public interface ICalculationService
    {
        Task<DrinkCalculationResult> CalculateStatsForUserAsync(string userId, Gender gender, int? weight);
    }
}
