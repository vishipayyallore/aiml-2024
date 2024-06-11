using cv_detect_people.Configuration;
using cv_detect_people.Extensions;
using HeaderFooter.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Azure.AI.Vision.ImageAnalysis;
using Azure;
using Azure.AI.Vision.Common;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
using Image = System.Drawing.Image;
using Font = System.Drawing.Font;

using IHost host = IHostExtensions.GetHostBuilder(args);

IHeader header = host.Services.GetRequiredService<IHeader>();
IFooter footer = host.Services.GetRequiredService<IFooter>();
AzAISvcAppConfiguration appConfig = host.Services.GetRequiredService<AzAISvcAppConfiguration>();

header.DisplayHeader('=', "Azure AI Services - CV Detect People");

try
{
    VisionServiceOptions cvClient = new(new Uri(appConfig.AiServicesEndpoint!), new AzureKeyCredential(appConfig.AiServicesKey!));

    // Get image
    string imageFile = "images/people3.jpg";
    if (args.Length > 0)
    {
        imageFile = args[0];
    }

    // Analyze image
    AnalyzeImage(imageFile, cvClient);

    ForegroundColor = ConsoleColor.DarkCyan;
}
catch (Exception ex)
{
    ForegroundColor = ConsoleColor.Red;
    WriteLine($"\n\nError: {ex.Message}");
    ResetColor();
}

footer.DisplayFooter('-');

ResetColor();
WriteLine("\n\nPress any key ...");
ReadKey();

static void AnalyzeImage(string imageFile, VisionServiceOptions serviceOptions)
{
    WriteLine($"\nAnalyzing {imageFile} \n");

    var analysisOptions = new ImageAnalysisOptions()
    {
        Features = ImageAnalysisFeature.People,
        GenderNeutralCaption = true,
        ModelVersion = "latest"
    };

    // Get image analysis
    using var imageSource = VisionSource.FromFile(imageFile);

    // Using the correct class from the Azure AI Vision package
    using var analyzer = new ImageAnalyzer(serviceOptions, imageSource, analysisOptions);

    var result = analyzer.Analyze();

    if (result.Reason == ImageAnalysisResultReason.Analyzed)
    {
        // Get people in the image
        if (result.People != null)
        {
            WriteLine($" People:");

            // Prepare image for drawing
            Image image = Image.FromFile(imageFile);
            Graphics graphics = Graphics.FromImage(image);
            Pen pen = new(Color.Cyan, 3);
            Font font = new("Arial", 16);
            SolidBrush brush = new(Color.WhiteSmoke);

            foreach (var person in result.People)
            {
                // Draw object bounding box if confidence > 50%
                if (person.Confidence > 0.5)
                {
                    // Draw object bounding box
                    var r = person.BoundingBox;
                    Rectangle rect = new(r.X, r.Y, r.Width, r.Height);
                    graphics.DrawRectangle(pen, rect);

                    // Return the confidence of the person detected
                    WriteLine($"   Bounding box {person.BoundingBox}, Confidence {person.Confidence:0.0000}");
                }
            }

            // Save annotated image
            String output_file = "./images/detected_people.jpg";
            image.Save(output_file);
            WriteLine("  Results saved in " + output_file + "\n");
        }
    }
    else
    {
        var errorDetails = ImageAnalysisErrorDetails.FromResult(result);
        Console.WriteLine(" Analysis failed.");
        Console.WriteLine($"   Error reason : {errorDetails.Reason}");
        Console.WriteLine($"   Error code : {errorDetails.ErrorCode}");
        Console.WriteLine($"   Error message: {errorDetails.Message}\n");
    }
}