namespace AAI.TextAnalyticsApp.Interfaces;

public interface ITextAnalyticsService
{
    Task<string> GetLanguage(string text);
}


