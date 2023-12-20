using AAI.TextAnalyticsApp.Interfaces;
using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.Extensions.Configuration;

namespace AAI.TextAnalyticsApp.Services;

public class TextAnalyticsService(IConfiguration configuration) : ITextAnalyticsService
{
    private readonly string _endpoint = configuration["AIServicesEndpoint"]!;
    private readonly string _key = configuration["AIServicesKey"]!;

    public async Task<string> GetLanguage(string text)
    {
        TextAnalyticsClient? client = new(new Uri(_endpoint), new AzureKeyCredential(_key));

        DetectedLanguage detectedLanguage = await client.DetectLanguageAsync(text);

        return detectedLanguage.Name;
    }
}