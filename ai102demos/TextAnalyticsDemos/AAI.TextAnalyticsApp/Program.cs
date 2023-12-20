using AAI.TextAnalyticsApp.Extensions;
using AAI.TextAnalyticsApp.Interfaces;
using AAI.TextAnalyticsApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
            {
                IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddUserSecrets("fb603ff5-AzAIServices")
                    .Build();

                services.ConfigureServices(configuration);
            })
            .Build();

ForegroundColor = ConsoleColor.DarkCyan;

// Now you can use the services provided by the host, including the configured IConfiguration
ITextAnalyticsService textAnalyticsService = host.Services.GetRequiredKeyedService<ITextAnalyticsService>(nameof(TextAnalyticsService));

try
{
    // Get user input (until they enter "quit")
    string userText = "";

    while (userText?.ToLower() != "quit")
    {
        WriteLine("\nEnter some text ('quit' to stop)");
        userText = Console.ReadLine()!;

        if (userText?.ToLower() != "quit")
        {
            // Call function to detect language
            string language = await textAnalyticsService.GetLanguage(userText!);

            WriteLine("Language: " + language);
        }
    }
}
catch (Exception exception)
{
    ForegroundColor = ConsoleColor.Red;

    WriteLine(exception.Message);
}
finally
{
    Console.ResetColor();
    Console.WriteLine("\n\nPress any key ...");
    Console.ReadKey();
}
