using System;
using System.Threading.Tasks;
using BingeBuddyNg.Functions.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BingeBuddyNg.Functions
{
    public class DrinkEventFunction
    {
        private readonly DrinkEventHandlingService drinkEventHandlingService;

        public DrinkEventFunction(
            DrinkEventHandlingService drinkEventHandlingService)
        {
            this.drinkEventHandlingService = drinkEventHandlingService ?? throw new ArgumentNullException(nameof(drinkEventHandlingService));
        }

        [FunctionName(nameof(DrinkEventFunction))]
        public async Task Run([TimerTrigger("0 */60 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"Drink Event executed at: {DateTime.Now}");
            await this.drinkEventHandlingService.TryCreateDrinkEventAsync();
        }
    }
}
