using AAI.TextAnalyticsApp.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using System.Web;

namespace AAI.TextAnalyticsApp.Services;

public class TextAnalyticsServiceRest(IConfiguration configuration) : ITextAnalyticsService
{
    private readonly string _endpoint = configuration["AIServicesEndpoint"]!;
    private readonly string _key = configuration["AIServicesKey"]!;
    private string _language = "";

    public async Task<string> GetLanguage(string text)
    {
        try
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

            UTF8Encoding utf8 = new(true, true);
            byte[] encodedBytes = utf8.GetBytes(jsonBody.ToString());

            //WriteLine(utf8.GetString(encodedBytes, 0, encodedBytes.Length));

            HttpClient client = new();
            NameValueCollection? queryString = HttpUtility.ParseQueryString(string.Empty);

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _key);

            string? uri = $"{_endpoint}text/analytics/v3.1/languages?{queryString}";

            HttpResponseMessage response;
            using (ByteArrayContent? content = new(encodedBytes))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response = await client.PostAsync(uri, content);
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var results = JsonObject.Parse(responseContent);
                //WriteLine(results?.ToString());

                // Extract the detected language name for each document
                foreach (JsonNode? document in results["documents"] as JsonArray)
                {
                    _language = document["detectedLanguage"]["name"]?.ToString() ?? "";
                    //WriteLine($"\nLanguage: {_language}");
                }
            }
            else
            {
                // Something went wrong, write the whole response
                WriteLine(response.ToString());
            }
        }
        catch (Exception ex)
        {
            WriteLine(ex.Message);
        }

        return _language;
    }
}
