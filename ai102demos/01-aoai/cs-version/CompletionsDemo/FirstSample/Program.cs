using Azure;
using FirstSample.Configuration;
using FirstSample.Extensions;
using HeaderFooter.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI;
using System.ClientModel;
using System.Net;

using IHost host = IHostExtensions.GetHostBuilder(args);

IHeader header = host.Services.GetRequiredService<IHeader>();
IFooter footer = host.Services.GetRequiredService<IFooter>();
AzAISvcAppConfiguration appConfig = host.Services.GetRequiredService<AzAISvcAppConfiguration>();

header.DisplayHeader('=', "Azure OpenAI Completions - Sample 1");

try
{
    OpenAIClientOptions openAIClientOptions = new()
    {
        Endpoint = new Uri(appConfig.AzureOpenAiEndpoint!)
    };

    var client = new OpenAIClient(new ApiKeyCredential(appConfig.AzureOpenAiKey), openAIClientOptions);

    const string prompt = "What are the top 10 countries with highest populations are along with their population count and capital city : \n";

    CompletionsOptions completionsOptions = new()
    {
        DeploymentName = "gpt-35-turbo-instruct",
        Prompts = { "When was Microsoft founded?" },
    };

    Response<Completions> completionsResponse = client.GetCompletions(completionsOptions);
    string completion = completionsResponse.Value.Choices[0].Text;
    Console.WriteLine($"Chatbot: {completion}");
}
catch (Exception)
{

    throw;
}
footer.DisplayFooter('-');

ResetColor();
WriteLine("\n\nPress any key ...");
ReadKey();