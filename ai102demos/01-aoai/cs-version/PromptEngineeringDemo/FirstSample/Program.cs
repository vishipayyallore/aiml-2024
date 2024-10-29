using Azure;
using Azure.AI.OpenAI;
using FirstSample.Configuration;
using FirstSample.Extensions;
using HeaderFooter.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

using IHost host = IHostExtensions.GetHostBuilder(args);

IHeader header = host.Services.GetRequiredService<IHeader>();
IFooter footer = host.Services.GetRequiredService<IFooter>();
AzAISvcAppConfiguration appConfig = host.Services.GetRequiredService<AzAISvcAppConfiguration>();
bool printFullResponse = false;


// Get prompt for image to be generated
Console.Clear();
Console.WriteLine("Enter a prompt to request an image:");
string prompt = Console.ReadLine() ?? "";

// Call the DALL-E model
using (var client = new HttpClient())
{
    var contentType = new MediaTypeWithQualityHeaderValue("application/json");
    var api = "openai/deployments/dall-e-3-dname/images/generations?api-version=2024-02-15-preview";
    client.BaseAddress = new Uri(appConfig.AzureOpenAiEndpoint);
    client.DefaultRequestHeaders.Accept.Add(contentType);
    client.DefaultRequestHeaders.Add("api-key", appConfig.AzureOpenAiKey);
    var data = new
    {
        prompt = prompt,
        n = 1,
        size = "1024x1024"
    };

    var jsonData = JsonSerializer.Serialize(data);
    var contentData = new StringContent(jsonData, Encoding.UTF8, "application/json");
    var response = await client.PostAsync(api, contentData);

    // Get the revised prompt and image URL from the response
    var stringResponse = await response.Content.ReadAsStringAsync();
    JsonNode contentNode = JsonNode.Parse(stringResponse)!;
    JsonNode dataCollectionNode = contentNode!["data"];
    JsonNode dataNode = dataCollectionNode[0]!;
    JsonNode revisedPrompt = dataNode!["revised_prompt"];
    JsonNode url = dataNode!["url"];
    Console.WriteLine(revisedPrompt.ToJsonString());
    Console.WriteLine(url.ToJsonString().Replace(@"\u0026", "&"));
}

header.DisplayHeader('=', "Azure OpenAI DALLE-3");

await ShowDalleDemo(appConfig);

footer.DisplayFooter('-');

header.DisplayHeader('=', "Azure OpenAI Chat Completion - Sample 1");

await ShowChatCompletionsDemo(appConfig, printFullResponse);

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

async Task ShowChatCompletionsDemo(AzAISvcAppConfiguration appConfig, bool printFullResponse)
{
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
}

static async Task ShowDalleDemo(AzAISvcAppConfiguration appConfig)
{


    OpenAIClient client = new(new Uri(appConfig.AzureOpenAiEndpoint!), new AzureKeyCredential(appConfig.AzureOpenAiKey!));

    Response<ImageGenerations> imageGenerations = await client.GetImageGenerationsAsync(
        new ImageGenerationOptions()
        {
            DeploymentName = "dall-e-3-dname",
            Prompt = "A Lion eating Apple, and Green Beans.",
            Size = ImageSize.Size1024x1024,
            ImageCount = 1
        });

    // Image Generations responses provide URLs you can use to retrieve requested images
    Uri imageUri = imageGenerations.Value.Data[0].Url;
}
