namespace AAI.TextAnalyticsApp.Interfaces
{
    public interface ITextAnalyticsRequestBuilder
    {
        HttpRequestMessage BuildLanguageDetectionRequest(string text);
    }
}