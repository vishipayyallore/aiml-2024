using analyze_faces.Configuration;
using analyze_faces.Extensions;
using HeaderFooter.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System.Drawing;

using IHost host = IHostExtensions.GetHostBuilder(args);

IHeader header = host.Services.GetRequiredService<IHeader>();
IFooter footer = host.Services.GetRequiredService<IFooter>();
AzAISvcAppConfiguration appConfig = host.Services.GetRequiredService<AzAISvcAppConfiguration>();
FaceClient faceClient;

header.DisplayHeader('=', "Azure AI Services - Analyze Faces");

try
{
    // Authenticate Face client
    ApiKeyServiceClientCredentials credentials = new(appConfig.AiServicesKey);
    faceClient = new FaceClient(credentials)
    {
        Endpoint = appConfig.AiServicesEndpoint
    };

    // Menu for face functions
    Console.WriteLine("1: Detect faces people.jpg,\n2: Detect faces people4.jpg,\nAny other key to quit");
    Console.WriteLine("Enter a number:");
    string command = ReadLine()!;
    switch (command)
    {
        case "1":
            await DetectFaces("images/people.jpg");
            break;
        case "2":
            await DetectFaces("images/people4.jpg");
            break;
        default:
            break;
    }
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

async Task DetectFaces(string imageFile)
{
    WriteLine($"Detecting faces in {imageFile}");

    // Specify facial features to be retrieved
    IList<FaceAttributeType> features =
    [
         FaceAttributeType.Occlusion,
         FaceAttributeType.Blur,
         FaceAttributeType.Glasses
    ];

    // Get faces
    using var imageData = File.OpenRead(imageFile);
    var detected_faces = await faceClient.Face.DetectWithStreamAsync(imageData, returnFaceAttributes: features, returnFaceId: false);

    if (detected_faces.Count() > 0)
    {
        Console.WriteLine($"{detected_faces.Count()} faces detected.");

        // Prepare image for drawing
        Image image = Image.FromFile(imageFile);
        Graphics graphics = Graphics.FromImage(image);
        Pen pen = new Pen(Color.LightGreen, 3);
        Font font = new Font("Arial", 4);
        SolidBrush brush = new SolidBrush(Color.White);
        int faceCount = 0;

        // Draw and annotate each face
        foreach (var face in detected_faces)
        {
            faceCount++;
            Console.WriteLine($"\nFace number {faceCount}");

            // Get face properties
            Console.WriteLine($" - Mouth Occluded: {face.FaceAttributes.Occlusion.MouthOccluded}");
            Console.WriteLine($" - Eye Occluded: {face.FaceAttributes.Occlusion.EyeOccluded}");
            Console.WriteLine($" - Blur: {face.FaceAttributes.Blur.BlurLevel}");
            Console.WriteLine($" - Glasses: {face.FaceAttributes.Glasses}");

            // Draw and annotate face
            var r = face.FaceRectangle;
            Rectangle rect = new Rectangle(r.Left, r.Top, r.Width, r.Height);
            graphics.DrawRectangle(pen, rect);
            string annotation = $"Face number {faceCount}";
            graphics.DrawString(annotation, font, brush, r.Left, r.Top);
        }

        // Save annotated image
        String output_file = "./images/detected_faces.jpg";
        image.Save(output_file);
        Console.WriteLine(" Results saved in " + output_file);
    }
}
