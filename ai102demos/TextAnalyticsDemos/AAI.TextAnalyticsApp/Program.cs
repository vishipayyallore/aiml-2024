using AAI.TextAnalyticsApp.Common;
using AAI.TextAnalyticsApp.Configuration;
using AAI.TextAnalyticsApp.Extensions;
using AAI.TextAnalyticsApp.Interfaces;
using AAI.TextAnalyticsApp.Services;
using HeaderFooter;
using HeaderFooter.Interfaces;
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

                TextAnalyticsAppConfiguration appConfig = new();
                configuration.GetSection("TextAnalyticsApp").Bind(appConfig);

                services.AddSingleton(appConfig);

                services.ConfigureServices();
            })
            .Build();

// Now you can use the services provided by the host, including the configured IConfiguration
ITextAnalyticsService textAnalyticsService = host.Services.GetRequiredKeyedService<ITextAnalyticsService>(nameof(TextAnalyticsService));
ITextAnalyticsService textAnalyticsServiceRest = host.Services.GetRequiredKeyedService<ITextAnalyticsService>(nameof(TextAnalyticsServiceRest));
IHeader header = host.Services.GetRequiredService<IHeader>();
IFooter footer = host.Services.GetRequiredService<IFooter>();

try
{
    // Get user input (until they enter "quit")
    string userText = "";

    header.DisplayHeader('=', "Azure AI Services - Text Analytics");

    while (userText?.ToLower() != "quit")
    {
        ForegroundColor = ConsoleColor.DarkCyan;
        
        WriteLine("\nEnter some text for Language Detection using Azure AI Services ('quit' to stop)");
        userText = Console.ReadLine()!;

        if (userText?.ToLower() != Constants.QuitCommand)
        {
            WriteLine($"\nCalling Azure AI Services with SDK ... with given {userText}");

            string language = await textAnalyticsService.GetLanguage(userText!);

            WriteLine("Language Detected using SDK: " + language);

            WriteLine($"\nCalling Azure AI Services with REST API ... with given {userText}");

            language = await textAnalyticsServiceRest.GetLanguage(userText!);

            WriteLine("Language Detected using REST API: " + language);
        }
    }

    footer.DisplayFooter('-');
}
catch (Exception exception)
{
    WriteLine(exception.Message);
}
finally
{
    Console.ResetColor();
    Console.WriteLine("\n\nPress any key ...");
    Console.ReadKey();
}
