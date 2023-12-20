using AAI.TextAnalyticsApp.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using System.Web;

namespace AAI.TextAnalyticsApp.Services;

internal class TextAnalyticsServiceRest(IConfiguration configuration) : ITextAnalyticsService
{
    private readonly string _endpoint = configuration["AIServicesEndpoint"]!;
    private readonly string _key = configuration["AIServicesKey"]!;

    public async Task<string> GetLanguage(string text)
    {

        try
        {
            // Create a collection of documents (we'll only use one, but you could have more)
            var documentsArray = new JsonArray
            {
                new JsonObject
                {
                    // Each document needs a unique ID and some text
                    { "id", 1 },
                    { "text", text }
                }
            };

            // Create a JSON object with the documents array
            var jsonBody = new JsonObject
            {
                { "documents", documentsArray }
            };

            // Encode as UTF-8
            var utf8 = new UTF8Encoding(true, true);
            var encodedBytes = utf8.GetBytes(jsonBody.ToString());

            // Let's take a look at the JSON we'll send to the service
            Console.WriteLine(utf8.GetString(encodedBytes, 0, encodedBytes.Length));

            // Make an HTTP request to the REST interface
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Add the authentication key to the header
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _key);

            // Use the endpoint to access the Text Analytics language API
            var uri = $"{_endpoint}text/analytics/v3.1/languages?{queryString}";

            // Send the request and get the response
            HttpResponseMessage response;
            using (var content = new ByteArrayContent(encodedBytes))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
            }

            // If the call was successful, get the response
            if (response.StatusCode == HttpStatusCode.OK)
            {
                // Display the JSON response in full (just so we can see it)
                var responseContent = await response.Content.ReadAsStringAsync();
                var results = JsonObject.Parse(responseContent);
                Console.WriteLine(results.ToString());

                // Extract the detected language name for each document
                foreach (var document in results["documents"] as JsonArray)
                {
                    Console.WriteLine($"\nLanguage: {document["detectedLanguage"]["name"]}");
                }
            }
            else
            {
                // Something went wrong, write the whole response
                Console.WriteLine(response.ToString());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return "";
    }
}
