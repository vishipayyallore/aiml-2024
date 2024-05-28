using Azure;
using Azure.AI.Vision.ImageAnalysis;
using HeaderFooter.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using read_text.Configuration;
using read_text.Extensions;
using System.Drawing;
using Image = System.Drawing.Image;

using IHost host = GetHostBuilder(args);

IHeader header = host.Services.GetRequiredService<IHeader>();
IFooter footer = host.Services.GetRequiredService<IFooter>();
AzAISvcAppConfiguration appConfig = host.Services.GetRequiredService<AzAISvcAppConfiguration>();

header.DisplayHeader('=', "Azure AI Services - OCR Read Text");

try
{
    ImageAnalysisClient client = new(new Uri(appConfig.AiServicesEndpoint!), new AzureKeyCredential(appConfig.AiServicesKey!));

    ForegroundColor = ConsoleColor.DarkCyan;

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
    ImageAnalysisResult result = client.Analyze(
        BinaryData.FromStream(stream),
        // Specify the features to be retrieved
        VisualFeatures.Read);

    stream.Close();

    // Display analysis results
    if (result.Read != null)
    {
        WriteLine($"Text:");

        // Prepare image for drawing
        Image image = Image.FromFile(imageFile);
        Graphics graphics = Graphics.FromImage(image);
        Pen pen = new Pen(Color.Cyan, 3);

        foreach (var line in result.Read.Blocks.SelectMany(block => block.Lines))
        {
            ForegroundColor = ConsoleColor.DarkGreen;

            // Return the text detected in the image
            WriteLine($"   '{line.Text}'");

            // Draw bounding box around line
            var drawLinePolygon = true;

            // Return the position bounding box around each line
            WriteLine($"   Bounding Polygon: [{string.Join(" ", line.BoundingPolygon)}]");

            ForegroundColor = ConsoleColor.DarkYellow;

            // Return each word detected in the image and the position bounding box around each word with the confidence level of each word
            foreach (DetectedTextWord word in line.Words)
            {
                Console.WriteLine($"     Word: '{word.Text}', Confidence {word.Confidence:F4}, Bounding Polygon: [{string.Join(" ", word.BoundingPolygon)}]");

                // Draw word bounding polygon
                drawLinePolygon = false;
                var r = word.BoundingPolygon;

                Point[] polygonPoints = [
                    new (r[0].X, r[0].Y),
                    new (r[1].X, r[1].Y),
                    new (r[2].X, r[2].Y),
                    new (r[3].X, r[3].Y)
                ];

                graphics.DrawPolygon(pen, polygonPoints);
            }

            // Draw line bounding polygon
            if (drawLinePolygon)
            {
                var r = line.BoundingPolygon;

                Point[] polygonPoints = [
                 new(r[0].X, r[0].Y),
                 new(r[1].X, r[1].Y),
                 new(r[2].X, r[2].Y),
                 new(r[3].X, r[3].Y)
                ];

                graphics.DrawPolygon(pen, polygonPoints);
            }

        }

        // Save image
        String output_file = "./images/text.jpg";
        image.Save(output_file);

        WriteLine("\nResults saved in " + output_file + "\n");
    }
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