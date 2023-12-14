using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.Extensions.Configuration;

IConfigurationRoot? _configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddUserSecrets("fb603ff5-AzAIServices")
    .Build();

ForegroundColor = ConsoleColor.DarkCyan;

// NOTE: Uncomment only to verify the values. ***** (Warning)
//WriteLine(_configuration["AIServicesEndpoint"]);
//WriteLine(_configuration["AIServicesKey"]);

try
{

}
catch (Exception exception)
{
    ForegroundColor = ConsoleColor.Red;

    WriteLine(exception.Message);
}

ResetColor();

WriteLine("\n\nPress any key ...");
ReadKey();

static string GetLanguage(string text, string AISvcEndpoint, string AISvcKey)
{

    // Create client using endpoint and key
    AzureKeyCredential credentials = new(AISvcKey);
    Uri endpoint = new(AISvcEndpoint);

    TextAnalyticsClient? client = new(endpoint, credentials);

    // Call the service to get the detected language
    DetectedLanguage detectedLanguage = client.DetectLanguage(text);

    return (detectedLanguage.Name);
}