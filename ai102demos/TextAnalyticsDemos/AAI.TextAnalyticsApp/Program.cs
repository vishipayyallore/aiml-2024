using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.Extensions.Configuration;

IConfigurationRoot? _configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddUserSecrets("fb603ff5-AzAIServices")
    .Build();

ForegroundColor = ConsoleColor.DarkCyan;

# region Output Data
// NOTE: Uncomment only to verify the values. ***** (Warning)
//WriteLine(_configuration["AIServicesEndpoint"]);
//WriteLine(_configuration["AIServicesKey"]);
#endregion

string AISvcEndpoint = _configuration["AIServicesEndpoint"]!;
string AISvcKey = _configuration["AIServicesKey"]!;

try
{
    // Get user input (until they enter "quit")
    string userText = "";

    while (userText?.ToLower() != "quit")
    {
        Console.WriteLine("\nEnter some text ('quit' to stop)");
        userText = Console.ReadLine()!;

        if (userText?.ToLower() != "quit")
        {
            // Call function to detect language
            string language = GetLanguage(userText!);
            Console.WriteLine("Language: " + language);
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

string GetLanguage(string text)
{

    AzureKeyCredential credentials = new(AISvcKey);
    Uri endpoint = new(AISvcEndpoint);

    TextAnalyticsClient? client = new(endpoint, credentials);

    DetectedLanguage detectedLanguage = client.DetectLanguage(text);

    return (detectedLanguage.Name);
}