using AAI.TextAnalyticsApp.Services;
using Microsoft.Extensions.Configuration;

IConfigurationRoot? _configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddUserSecrets("fb603ff5-AzAIServices")
    .Build();

ForegroundColor = ConsoleColor.DarkCyan;

TextAnalyticsService textAnalyticsService = new(_configuration);

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
            string language = textAnalyticsService.GetLanguage(userText!);

            WriteLine("Language: " + language);
        }
    }
}
catch (Exception exception)
{
    ForegroundColor = ConsoleColor.Red;

    WriteLine(exception.Message);
}

ResetColor();

WriteLine("\n\nPress any key ...");
ReadKey();
