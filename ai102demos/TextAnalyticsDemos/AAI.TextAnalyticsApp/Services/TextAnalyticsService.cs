using AAI.TextAnalyticsApp.Interfaces;
using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.Extensions.Configuration;

namespace AAI.TextAnalyticsApp.Services;

public class TextAnalyticsService : ITextAnalyticsService
{
    //private readonly string _endpoint = configuration["AIServicesEndpoint"]!;
    //private readonly string _key = configuration["AIServicesKey"]!;

    private readonly string _endpoint;
    private readonly string _key;

    public TextAnalyticsService(IConfiguration configuration)
    {
        Console.WriteLine("Initializing TextAnalyticsService...");

        _endpoint = configuration["AIServicesEndpoint"]!;
        _key = configuration["AIServicesKey"]!;

        Console.WriteLine($"Endpoint: {_endpoint}");
        Console.WriteLine($"Key: {_key}");
    }

    public string GetLanguage(string text)
    {
        AzureKeyCredential credentials = new(_key);
        Uri endpoint = new(_endpoint);

        TextAnalyticsClient? client = new(endpoint, credentials);

        DetectedLanguage detectedLanguage = client.DetectLanguage(text);

        return detectedLanguage.Name;
    }
}