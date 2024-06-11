using cv_detect_people.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace cv_detect_people.Extensions;

public static class IHostExtensions
{
    public static IHost GetHostBuilder(string[] args)
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
}
