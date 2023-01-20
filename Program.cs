using Azure.Messaging.EventGrid;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration.Extensions;
using System;
using System.Threading.Tasks;

namespace TestConsole
{

    class Program
    {
        private const string AppConfigurationConnectionString = "Populate Endpoint here to app store";
        private static IConfiguration configuration = null;
        private static IConfigurationRefresher _refresher = null;

        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddAzureAppConfiguration(options =>
            {
                options.Connect(AppConfigurationConnectionString)
                            .Select(KeyFilter.Any);

                options.UseFeatureFlags();

                _refresher = options.GetRefresher();
            });

            configuration = builder.Build();
            var test1Message = configuration["FeatureManagement:TEST"];
            while (true)
            {
                Console.WriteLine($"Existing values: Test: {configuration["FeatureManagement:TEST"]}");
                if (configuration["FeatureManagement:TEST"] != test1Message)
                {
                    Console.WriteLine($"Value Now: {configuration["FeatureManagement:TEST"]}");
                    test1Message = configuration["FeatureManagement:TEST"];
                }

                Console.WriteLine($"Waiting 10 seconds");
                await Task.Delay(TimeSpan.FromSeconds(10));

                await _refresher.RefreshAsync();
            }
        }
    }
}

