using AAI.TextAnalyticsApp.Interfaces;
using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.Extensions.Configuration;

namespace AAI.TextAnalyticsApp.Services;

public class TextAnalyticsService(IConfigurationRoot configuration) : ITextAnalyticsService
{
    private readonly string _endpoint = configuration["AIServicesEndpoint"]!;
    private readonly string _key = configuration["AIServicesKey"]!;

    public string GetLanguage(string text)
    {
        AzureKeyCredential credentials = new(_key);
        Uri endpoint = new(_endpoint);

        TextAnalyticsClient? client = new(endpoint, credentials);

        DetectedLanguage detectedLanguage = client.DetectLanguage(text);

        return detectedLanguage.Name;
    }
}