using Azure;
using Azure.AI.Vision.ImageAnalysis;
using HeaderFooter.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using read_text.Configuration;
using read_text.Extensions;

using IHost host = GetHostBuilder(args);

IHeader header = host.Services.GetRequiredService<IHeader>();
IFooter footer = host.Services.GetRequiredService<IFooter>();
AzAISvcAppConfiguration appConfig = host.Services.GetRequiredService<AzAISvcAppConfiguration>();

header.DisplayHeader('=', "Azure AI Services - OCR Read Text");

try
{
    ImageAnalysisClient client = new(new Uri(appConfig.AiServicesEndpoint!), new AzureKeyCredential(appConfig.AiServicesKey!));

    // Menu for text reading functions
    WriteLine("\n1: Use Read API for image (Lincoln.jpg)\n2: Read handwriting (Note.jpg)\nAny other key to quit\n");
    WriteLine("Enter a number:");
    string? command = ReadLine();
    string imageFile;

    switch (command)
    {
        case "1":
            imageFile = "images/Lincoln.jpg";
            GetTextRead(imageFile, client);
            break;
        case "2":
            imageFile = "images/Note.jpg";
            GetTextRead(imageFile, client);
            break;
        default:
            break;
    }
}
catch (Exception ex)
{
    WriteLine($"\nError: {ex.Message}");
}

footer.DisplayFooter('-');

ResetColor();
WriteLine("\n\nPress any key ...");
ReadKey();

static void GetTextRead(string imageFile, ImageAnalysisClient client)
{
    WriteLine($"\nReading text from {imageFile} \n");

    // Use a file stream to pass the image data to the analyze call
    using FileStream stream = new(imageFile, FileMode.Open);

    // Use Analyze image function to read text in image
}

static IHost GetHostBuilder(string[] args)
{
    return Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                {
                    IConfiguration configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddUserSecrets("fb603ff5-AzAIServices")
                        .Build();

                    AzAISvcAppConfiguration appConfig = new();
                    configuration.GetSection("AzAISvcAppConfiguration").Bind(appConfig);

                    services.AddSingleton(appConfig);

                    services.ConfigureServices();
                })
                .Build();
}