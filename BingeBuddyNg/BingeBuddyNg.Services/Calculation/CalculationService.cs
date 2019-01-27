using System;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.User;


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

            var orderedAlcoholicDrinks = userDrinkActivity.Drinks.Where(d=>d.AlcPrc > 0).OrderBy(d => d.Timestamp).ToList();
            orderedAlcoholicDrinks.Add(new DrinkActivityItem(DateTime.UtcNow));

            double currentAlcoholization = 0.0;
            var factor = userDrinkActivity.Gender == Gender.Female ? 0.61 : 0.69;

            for (int i = 0; i < orderedAlcoholicDrinks.Count; i++)
            {
                var d = orderedAlcoholicDrinks[i];

                var tmpPegel = (d.VolMl * d.AlcPrc / 100 * 0.8) / (userDrinkActivity.Weight * factor);

                if (i > 0)
                {
                    var timeSinceLastDrink = (d.Timestamp - orderedAlcoholicDrinks[i - 1].Timestamp);
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

            return new DrinkCalculationResult(userDrinkActivity.UserId, currentAlcoholization, orderedAlcoholicDrinks.Count-1);
        }

        public async Task<DrinkCalculationResult> CalculateStatsForUserAsync(User.User user)
        {
            DateTime startTimestamp = DateTime.UtcNow.Subtract(TimeSpan.FromHours(12));
            var activity = await this.ActivityRepository.GetActivitysForUserAsync(user.Id, startTimestamp,
                ActivityType.Drink);

            var drinkActivity = activity.Where(a=>a.Timestamp >= startTimestamp).Select(a => new DrinkActivityItem(a.Timestamp, a.DrinkAlcPrc.GetValueOrDefault(),
                a.DrinkVolume.GetValueOrDefault()));
                        
            UserDrinkActivity userDrinkActivity = new UserDrinkActivity(user.Id, user.Gender, user.Weight.GetValueOrDefault(), drinkActivity);

            DrinkCalculationResult result = CalculateStats(userDrinkActivity);
            
            return result;
        }
    }
}
