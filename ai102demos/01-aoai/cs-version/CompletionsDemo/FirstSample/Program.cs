using Azure;
using FirstSample.Configuration;
using FirstSample.Extensions;
using HeaderFooter.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Azure.AI.OpenAI;
using System.Net;

using IHost host = IHostExtensions.GetHostBuilder(args);

IHeader header = host.Services.GetRequiredService<IHeader>();
IFooter footer = host.Services.GetRequiredService<IFooter>();
AzAISvcAppConfiguration appConfig = host.Services.GetRequiredService<AzAISvcAppConfiguration>();

header.DisplayHeader('=', "Azure OpenAI Completions - Sample 1");

try
{
    ForegroundColor = ConsoleColor.DarkCyan;
    const string prompt = "When was Microsoft founded?";

    OpenAIClient client = new(new Uri(appConfig.AzureOpenAiEndpoint!), new AzureKeyCredential(appConfig.AzureOpenAiKey!));

    CompletionsOptions completionsOptions = new()
    {
        DeploymentName = "gpt-35-turbo-dname",
        Prompts = { $"{prompt}" },
        Echo = true,
        MaxTokens = 150,
        Temperature = 0
    };

    WriteLine($"User Prompt: {prompt}");

    Response<Completions> completionsResponse = client.GetCompletions(completionsOptions);
    WriteLine($"Response Received: {completionsResponse.Value.Choices}");

    string completion = completionsResponse.Value.Choices[0].Text;
    WriteLine($"\n\nChatbot: {completion}");

    ResetColor();
}
catch (Exception)
{

    throw;
}
footer.DisplayFooter('-');

ResetColor();
WriteLine("\n\nPress any key ...");
ReadKey();