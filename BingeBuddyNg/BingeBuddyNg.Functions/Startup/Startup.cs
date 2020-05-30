using BingeBuddyNg.Functions;
using BingeBuddyNg.Functions.Services;
using BingeBuddyNg.Services.Activity;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace BingeBuddyNg.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;
            
            services.AddBingeBuddy();

            services.AddScoped<ActivityAddedService>();
            services.AddScoped<ActivityDistributionService>();
            services.AddScoped<DrinkEventHandlingService>();
            services.AddScoped<PushNotificationService>();
        }
    }
}
