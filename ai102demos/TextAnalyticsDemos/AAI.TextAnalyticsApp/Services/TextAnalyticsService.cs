using AAI.TextAnalyticsApp.Configuration;
using AAI.TextAnalyticsApp.Interfaces;
using Azure;
using Azure.AI.TextAnalytics;

namespace AAI.TextAnalyticsApp.Services;

public class TextAnalyticsService(TextAnalyticsAppConfiguration appConfig) : ITextAnalyticsService
{
    private readonly string _endpoint = appConfig?.AiServicesEndpoint!;
    private readonly string _key = appConfig?.AiServicesKey!;

    public async Task<string> GetLanguage(string text)
    {
        TextAnalyticsClient? client = new(new Uri(_endpoint), new AzureKeyCredential(_key));

        DetectedLanguage detectedLanguage = await client.DetectLanguageAsync(text);

        return detectedLanguage.Name;
    }
}