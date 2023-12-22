using AAI.TextAnalyticsApp.Configuration;
using AAI.TextAnalyticsApp.Interfaces;
using System.Collections.Specialized;
using System.Text.Json.Nodes;
using System.Web;

namespace AAI.TextAnalyticsApp.Services;

public class TextAnalyticsRequestBuilder(TextAnalyticsAppConfiguration appConfig) : ITextAnalyticsRequestBuilder
{
    private readonly string _endpoint = appConfig?.AiServicesEndpoint ?? string.Empty;
    private readonly string _key = appConfig?.AiServicesKey ?? string.Empty;

    public HttpRequestMessage BuildLanguageDetectionRequest(string text)
    {
        JsonArray documentsArray =
            [
                new JsonObject
                {
                    { "id", 1 },
                    { "text", text }
                }
            ];

        JsonObject jsonBody = new()
        {
            { "documents", documentsArray }
        };

        JsonContent jsonContent = new(jsonBody.ToString());

        HttpRequestMessage request = new()
        {
            Method = HttpMethod.Post,
            Content = jsonContent,
            RequestUri = new Uri($"{_endpoint}text/analytics/v3.1/languages?{GetQueryString()}")
        };

        request.Headers.Add("Ocp-Apim-Subscription-Key", _key);

        return request;
    }

    private static string GetQueryString()
    {
        NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);

        // Add any additional query parameters if needed
        return queryString.ToString() ?? string.Empty;
    }
}