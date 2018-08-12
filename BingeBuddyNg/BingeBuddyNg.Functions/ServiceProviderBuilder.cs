using BingeBuddyNg.Functions.DependencyInjection;
using BingeBuddyNg.Services;
using BingeBuddyNg.Services.Configuration;
using BingeBuddyNg.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace BingeBuddyNg.Functions
{
    /// <summary>
    /// Builds the dependency injection container and registers all services.
    /// </summary>
    public class ServiceProviderBuilder : IServiceProviderBuilder
    {        
        public IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();

            string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process);
            string googleApiKey = Environment.GetEnvironmentVariable("GoogleApiKey", EnvironmentVariableTarget.Process);

            services.AddHttpClient();

            services.AddSingleton<AppConfiguration>(new AppConfiguration(storageConnectionString, googleApiKey));
            services.AddScoped<StorageAccessService>();
            services.AddScoped<IActivityRepository, ActivityRepository>();
            services.AddScoped<IUtilityService, UtilityService>();
                                    
            return services.BuildServiceProvider(true);
        }
    }
}
