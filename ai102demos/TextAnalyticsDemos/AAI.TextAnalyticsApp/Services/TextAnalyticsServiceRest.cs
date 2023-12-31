﻿using AAI.TextAnalyticsApp.Interfaces;
using System.Text.Json.Nodes;

namespace AAI.TextAnalyticsApp.Services;

public class TextAnalyticsServiceRest(ITextAnalyticsRequestBuilder textAnalyticsRequestBuilder, IHttpClientFactory httpClientFactory) : ITextAnalyticsService
{
    private readonly ITextAnalyticsRequestBuilder _textAnalyticsRequestBuilder = textAnalyticsRequestBuilder;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private string _language = "";

    public async Task<string> GetLanguage(string text)
    {
        try
        {
            using var client = _httpClientFactory.CreateClient();
            var request = _textAnalyticsRequestBuilder.BuildLanguageDetectionRequest(text);

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var results = JsonObject.Parse(responseContent);

                foreach (JsonNode document in (results["documents"] as JsonArray))
                {
                    _language = document["detectedLanguage"]["name"]?.ToString() ?? "";
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


//JsonArray documentsArray =
//[
//    new JsonObject
//                {
//                    { "id", 1 },
//                    { "text", text }
//                }
//];

//JsonObject jsonBody = new()
//            {
//    { "documents", documentsArray }
//            };

//UTF8Encoding utf8 = new(true, true);
//byte[] encodedBytes = utf8.GetBytes(jsonBody.ToString());

//            //WriteLine(utf8.GetString(encodedBytes, 0, encodedBytes.Length));

//            using (var client = _httpClientFactory.CreateClient())
//            {
//                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _key);

//                NameValueCollection? queryString = HttpUtility.ParseQueryString(string.Empty);
//string? uri = $"{_endpoint}text/analytics/v3.1/languages?{queryString}";

//HttpResponseMessage response;
//                using (ByteArrayContent? content = new (encodedBytes))
//                {
//                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

//response = await client.PostAsync(uri, content);
//                }

//                if (response.StatusCode == HttpStatusCode.OK)
//{
//    var responseContent = await response.Content.ReadAsStringAsync();
//    var results = JsonObject.Parse(responseContent);
//    //WriteLine(results?.ToString());

//    // Extract the detected language name for each document
//    foreach (JsonNode? document in results["documents"] as JsonArray)
//    {
//        _language = document["detectedLanguage"]["name"]?.ToString() ?? "";
//        //WriteLine($"\nLanguage: {_language}");
//    }
//}
//else
//{
//    // Something went wrong, write the whole response
//    WriteLine(response.ToString());
//}
//            }
