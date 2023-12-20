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

// Now you can use the services provided by the host, including the configured IConfiguration
ITextAnalyticsService textAnalyticsService = host.Services.GetRequiredKeyedService<ITextAnalyticsService>(nameof(TextAnalyticsService));
ITextAnalyticsService textAnalyticsServiceRest = host.Services.GetRequiredKeyedService<ITextAnalyticsService>(nameof(TextAnalyticsServiceRest));

try
{
    // Get user input (until they enter "quit")
    string userText = "";

    while (userText?.ToLower() != "quit")
    {
        ForegroundColor = ConsoleColor.DarkCyan;

        WriteLine("\nEnter some text ('quit' to stop)");
        userText = Console.ReadLine()!;

        if (userText?.ToLower() != "quit")
        {
            WriteLine($"Calling Azure Cognitive Services with SDK ... with given {userText}");

            string language = await textAnalyticsService.GetLanguage(userText!);

            WriteLine("Language Detected using SDK: " + language);

            ForegroundColor = ConsoleColor.Yellow;
            WriteLine($"Calling Azure Cognitive Services with REST API ... with given {userText}");

            language = await textAnalyticsServiceRest.GetLanguage(userText!);

            WriteLine("Language Detected using REST API: " + language);
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
