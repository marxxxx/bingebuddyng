using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Calculation
{
    public interface ICalculationService
    {
        Task<DrinkCalculationResult> CalculateStatsForUserAsync(User.User user);
    }
}
