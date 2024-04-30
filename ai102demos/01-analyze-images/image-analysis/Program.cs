﻿using Azure;
using Azure.AI.Vision.ImageAnalysis;
using HeaderFooter.Interfaces;
using imageanalysis.Configuration;
using imageanalysis.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
using Image = System.Drawing.Image;
using Font = System.Drawing.Font;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Net;

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

await BackgroundForeground(imageFile, appConfig.AiServicesEndpoint!, appConfig.AiServicesKey!);

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

    // Get image tags
    if (result.Tags.Values.Count > 0)
    {
        Console.WriteLine($"\n Tags:");
        foreach (DetectedTag tag in result.Tags.Values)
        {
            Console.WriteLine($"   '{tag.Name}', Confidence: {tag.Confidence:F2}");
        }
    }

    // Get objects in the image
    if (result.Objects.Values.Count > 0)
    {
        Console.WriteLine(" Objects:");

        // Prepare image for drawing
        stream.Close();
        Image image = Image.FromFile(imageFile);
        Graphics graphics = Graphics.FromImage(image);
        Pen pen = new Pen(Color.Cyan, 3);
        Font font = new Font("Arial", 16);
        SolidBrush brush = new SolidBrush(Color.WhiteSmoke);

        foreach (DetectedObject detectedObject in result.Objects.Values)
        {
            Console.WriteLine($"   \"{detectedObject.Tags[0].Name}\"");

            // Draw object bounding box
            var r = detectedObject.BoundingBox;
            Rectangle rect = new Rectangle(r.X, r.Y, r.Width, r.Height);
            graphics.DrawRectangle(pen, rect);
            graphics.DrawString(detectedObject.Tags[0].Name, font, brush, (float)r.X, (float)r.Y);
        }

        // Save annotated image
        String output_file = "objects.jpg";
        image.Save(output_file);
        Console.WriteLine("  Results saved in " + output_file + "\n");
    }

    // Get people in the image
    if (result.People.Values.Count > 0)
    {
        Console.WriteLine($" People:");

        // Prepare image for drawing
        System.Drawing.Image image = System.Drawing.Image.FromFile(imageFile);
        Graphics graphics = Graphics.FromImage(image);
        Pen pen = new Pen(Color.Cyan, 3);
        Font font = new Font("Arial", 16);
        SolidBrush brush = new SolidBrush(Color.WhiteSmoke);

        foreach (DetectedPerson person in result.People.Values)
        {
            // Draw object bounding box
            var r = person.BoundingBox;
            Rectangle rect = new Rectangle(r.X, r.Y, r.Width, r.Height);
            graphics.DrawRectangle(pen, rect);

            // Return the confidence of the person detected
            //Console.WriteLine($"   Bounding box {person.BoundingBox.ToString()}, Confidence: {person.Confidence:F2}");
        }

        // Save annotated image
        String output_file = "persons.jpg";
        image.Save(output_file);
        Console.WriteLine("  Results saved in " + output_file + "\n");
    }

}

static async Task BackgroundForeground(string imageFile, string endpoint, string key)
{
    // Remove the background from the image or generate a foreground matte
    Console.WriteLine($" Background removal:");
    // Define the API version and mode
    string apiVersion = "2023-02-01-preview";
    string mode = "backgroundRemoval"; // Can be "foregroundMatting" or "backgroundRemoval"

    string url = $"computervision/imageanalysis:segment?api-version={apiVersion}&mode={mode}";

    // Make the REST call
    using (var client = new HttpClient())
    {
        var contentType = new MediaTypeWithQualityHeaderValue("application/json");
        client.BaseAddress = new Uri(endpoint);
        client.DefaultRequestHeaders.Accept.Add(contentType);
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);

        // You can change the url to use other images in the images folder,
        // such as "building.jpg" or "person.jpg" to see different results.
        var data = new
        {
            url = "https://github.com/MicrosoftLearning/mslearn-ai-vision/blob/main/Labfiles/01-analyze-images/Python/image-analysis/images/street.jpg?raw=true"
        };

        var jsonData = JsonSerializer.Serialize(data);
        var contentData = new StringContent(jsonData, Encoding.UTF8, contentType);
        var response = await client.PostAsync(url, contentData);

        if (response.IsSuccessStatusCode)
        {
            File.WriteAllBytes("background.png", response.Content.ReadAsByteArrayAsync().Result);
            Console.WriteLine("  Results saved in background.png\n");
        }
        else
        {
            Console.WriteLine($"API error: {response.ReasonPhrase} - Check your body url, key, and endpoint.");
        }
    }

}