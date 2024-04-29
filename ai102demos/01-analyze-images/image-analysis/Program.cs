using Azure;
using Azure.AI.Vision.ImageAnalysis;
using HeaderFooter.Interfaces;
using imageanalysis.Configuration;
using imageanalysis.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
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

IHeader header = host.Services.GetRequiredService<IHeader>();
IFooter footer = host.Services.GetRequiredService<IFooter>();
AzAISvcAppConfiguration appConfig = host.Services.GetRequiredService<AzAISvcAppConfiguration>();

header.DisplayHeader('=', "Azure AI Services - Image Analysis");

// Get image
string imageFile = "images/street.jpg";
if (args.Length > 0)
{
    imageFile = args[0];
}

// Authenticate Azure AI Vision client
ImageAnalysisClient client = new(new Uri(appConfig.AiServicesEndpoint!), new AzureKeyCredential(appConfig.AiServicesKey!));

// Analyze image
AnalyzeImage(imageFile, client);

footer.DisplayFooter('-');

ResetColor();
WriteLine("\n\nPress any key ...");
ReadKey();

static void AnalyzeImage(string imageFile, ImageAnalysisClient client)
{
    WriteLine($"\nAnalyzing {imageFile} \n");

    // Use a file stream to pass the image data to the analyze call
    using FileStream stream = new(imageFile, FileMode.Open);

    // Get result with specified features to be retrieved
    ImageAnalysisResult result = client.Analyze(
        BinaryData.FromStream(stream),
        VisualFeatures.Caption |
        VisualFeatures.DenseCaptions |
        VisualFeatures.Objects |
        VisualFeatures.Tags |
        VisualFeatures.People);


    // Display analysis results

    // Get image captions
    if (result.Caption.Text != null)
    {
        WriteLine(" Caption:");
        WriteLine($"   \"{result.Caption.Text}\", Confidence {result.Caption.Confidence:0.00}\n");
    }

    // Get image dense captions
    WriteLine(" Dense Captions:");
    foreach (DenseCaption denseCaption in result.DenseCaptions.Values)
    {
        WriteLine($"   Caption: '{denseCaption.Text}', Confidence: {denseCaption.Confidence:0.00}");
    }

}