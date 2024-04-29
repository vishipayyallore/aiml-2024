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

                ImageAnalysisAppConfiguration appConfig = new();
                configuration.GetSection("TextAnalyticsApp").Bind(appConfig);

                services.AddSingleton(appConfig);

                services.ConfigureServices();
            })
            .Build();

IHeader header = host.Services.GetRequiredService<IHeader>();
IFooter footer = host.Services.GetRequiredService<IFooter>();

header.DisplayHeader('=', "Azure AI Services - Image Analysis");

footer.DisplayFooter('-');

ResetColor();
WriteLine("\n\nPress any key ...");
ReadKey();