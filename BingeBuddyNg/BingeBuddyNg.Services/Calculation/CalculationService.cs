using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Models;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace BingeBuddyNg.Services.Calculation
{
    public class CalculationService : ICalculationService
    {
        private const int DefaultWeight = 80;

        public IActivityRepository ActivityRepository { get; }

        public CalculationService(IActivityRepository activityRepository)
        {
            ActivityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        }


        private DrinkCalculationResult CalculateStats(UserDrinkActivity userDrinkActivity)
        {
            if (userDrinkActivity.Weight == 0)
                userDrinkActivity.Weight = DefaultWeight;

            var orderedDrinks = userDrinkActivity.Drinks.OrderBy(d => d.Timestamp).ToList();
            orderedDrinks.Add(new DrinkActivityItem(DateTime.UtcNow));

            double currentAlcoholization = 0.0;
            var factor = userDrinkActivity.Gender == Models.Gender.Female ? 0.61 : 0.69;

            for (int i = 0; i < orderedDrinks.Count; i++)
            {
                var d = orderedDrinks[i];

                var tmpPegel = (d.VolMl * d.AlcPrc / 100 * 0.8) / (userDrinkActivity.Weight * factor);

                if (i > 0)
                {
                    var timeSinceLastDrink = (d.Timestamp - orderedDrinks[i - 1].Timestamp);
                    var decay = timeSinceLastDrink.TotalMinutes * 0.002;

                    if (decay > currentAlcoholization)
                        currentAlcoholization = tmpPegel;
                    else
                        currentAlcoholization += (tmpPegel - decay);
                }
                else
                {
                    currentAlcoholization += tmpPegel;
                }
            }

            currentAlcoholization = Math.Round(currentAlcoholization, 3);

            return new DrinkCalculationResult(userDrinkActivity.UserId, currentAlcoholization, orderedDrinks.Count-1);
        }

        public async Task<DrinkCalculationResult> CalculateStatsForUserAsync(User user)
        {
            DateTime startTimestamp = DateTime.UtcNow.Subtract(TimeSpan.FromHours(12));
            var activity = await this.ActivityRepository.GetActivitysForUser(user.Id, startTimestamp,
                Models.ActivityType.Drink);

            var drinkActivity = activity.Where(a=>a.Timestamp >= startTimestamp).Select(a => new DrinkActivityItem(a.Timestamp, a.DrinkAlcPrc.GetValueOrDefault(),
                a.DrinkVolume.GetValueOrDefault()));
                        
            UserDrinkActivity userDrinkActivity = new UserDrinkActivity(user.Id, user.Gender, user.Weight.GetValueOrDefault(), drinkActivity);

            DrinkCalculationResult result = CalculateStats(userDrinkActivity);
            
            return result;
        }
    }
}
