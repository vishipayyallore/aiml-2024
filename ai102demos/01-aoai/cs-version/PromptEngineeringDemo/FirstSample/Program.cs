using Azure;
using Azure.AI.OpenAI;
using FirstSample.Configuration;
using FirstSample.Extensions;
using HeaderFooter.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

using IHost host = IHostExtensions.GetHostBuilder(args);

IHeader header = host.Services.GetRequiredService<IHeader>();
IFooter footer = host.Services.GetRequiredService<IFooter>();
AzAISvcAppConfiguration appConfig = host.Services.GetRequiredService<AzAISvcAppConfiguration>();
bool printFullResponse = false;

header.DisplayHeader('=', "Azure OpenAI Chat Completion - Sample 1");

try
{
    ForegroundColor = ConsoleColor.DarkCyan;

    OpenAIClient client = new(new Uri(appConfig.AzureOpenAiEndpoint!), new AzureKeyCredential(appConfig.AzureOpenAiKey!));

    do
    {
        // Pause for system message update
        Console.WriteLine("-----------\nPausing the app to allow you to change the system prompt.\nPress any key to continue...");
        Console.ReadKey();

        Console.WriteLine("\nUsing system message from system.txt");
        string systemMessage = System.IO.File.ReadAllText("system.txt");
        systemMessage = systemMessage.Trim();

        Console.WriteLine("\nEnter user message or type 'quit' to exit:");
        string userMessage = Console.ReadLine() ?? "";
        userMessage = userMessage.Trim();

        if (systemMessage.ToLower() == "quit" || userMessage.ToLower() == "quit")
        {
            break;
        }
        else if (string.IsNullOrEmpty(systemMessage) || string.IsNullOrEmpty(userMessage))
        {
            Console.WriteLine("Please enter a system and user message.");
            continue;
        }
        else
        {
            await GetResponseFromOpenAI(systemMessage, userMessage);
        }
    } while (true);

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

async Task GetResponseFromOpenAI(string systemMessage, string userMessage)
{
    Console.WriteLine("\nSending prompt to Azure OpenAI endpoint...\n\n");

    if (string.IsNullOrEmpty(appConfig.AzureOpenAiEndpoint) || string.IsNullOrEmpty(appConfig.AzureOpenAiKey) || string.IsNullOrEmpty(appConfig.AzureOpenAiDeploymentName))
    {
        Console.WriteLine("Please check your appsettings.json file for missing or incorrect values.");
        return;
    }

    // Configure the Azure OpenAI client


    // Format and send the request to the model


    ChatCompletions completions = response.Value;
    string completion = completions.Choices[0].Message.Content;

    // Write response full response to console, if requested
    if (printFullResponse)
    {
        Console.WriteLine($"\nFull response: {JsonSerializer.Serialize(completions, new JsonSerializerOptions { WriteIndented = true })}\n\n");
    }

    // Write response to console
    Console.WriteLine($"\nResponse:\n{completion}\n\n");
}