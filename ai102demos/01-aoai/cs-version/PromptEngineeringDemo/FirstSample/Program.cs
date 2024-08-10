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

    do
    {
        ForegroundColor = ConsoleColor.DarkCyan;

        // Pause for system message update
        WriteLine("-----------\nPausing the app to allow you to change the system prompt.\nPress any key to continue...");
        ReadKey();

        WriteLine("\nUsing system message from system.txt");
        string systemMessage = File.ReadAllText("system.txt");
        systemMessage = systemMessage.Trim();

        WriteLine("\nEnter user message or type 'quit' to exit:");
        string userMessage = ReadLine() ?? string.Empty;
        userMessage = userMessage.Trim();

        if (systemMessage.Equals("quit", StringComparison.CurrentCultureIgnoreCase) || userMessage.Equals("quit", StringComparison.CurrentCultureIgnoreCase))
        {
            break;
        }
        else if (string.IsNullOrEmpty(systemMessage) || string.IsNullOrEmpty(userMessage))
        {
            WriteLine("Please enter BOTH a system and user message.");
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
WriteLine("\n\nThank you for using Azure Open AI ... Press any key ...");
ReadKey();

async Task GetResponseFromOpenAI(string systemMessage, string userMessage)
{
    ForegroundColor = ConsoleColor.DarkGreen;

    WriteLine("\nSending prompt to Azure OpenAI endpoint...\n\n");

    if (string.IsNullOrEmpty(appConfig.AzureOpenAiEndpoint) || string.IsNullOrEmpty(appConfig.AzureOpenAiKey) || string.IsNullOrEmpty(appConfig.AzureOpenAiDeploymentName))
    {
        WriteLine("Please check your appsettings.json file for missing or incorrect values.");
        return;
    }

    // Configure the Azure OpenAI client
    OpenAIClient client = new(new Uri(appConfig.AzureOpenAiEndpoint!), new AzureKeyCredential(appConfig.AzureOpenAiKey!));

    // Format and send the request to the model
    WriteLine("\nAdding grounding context from grounding.txt");
    string groundingText = System.IO.File.ReadAllText("grounding.txt");
    userMessage = groundingText + userMessage;

    ChatCompletionsOptions chatCompletionsOptions = new()
    {
        Messages =
         {
             new ChatRequestSystemMessage(systemMessage),
             new ChatRequestUserMessage(userMessage)
         },
        Temperature = 0.7f,
        MaxTokens = 800,
        DeploymentName = appConfig.AzureOpenAiDeploymentName
    };

    // Get response from Azure OpenAI
    Response<ChatCompletions> response = await client.GetChatCompletionsAsync(chatCompletionsOptions);

    ChatCompletions completions = response.Value;
    string completion = completions.Choices[0].Message.Content;

    // Write response full response to console, if requested
    if (printFullResponse)
    {
        WriteLine($"\nFull response: {JsonSerializer.Serialize(completions, new JsonSerializerOptions { WriteIndented = true })}\n\n");
    }

    // Write response to console
    WriteLine($"\nResponse:\n{completion}\n\n");
}