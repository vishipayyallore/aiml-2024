using AAI.TextAnalyticsApp.Interfaces;
using AAI.TextAnalyticsApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AAI.TextAnalyticsApp.Extensions;

public static class AAITextAnalyticsAppExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register the IConfiguration instance
        services.AddSingleton(configuration);

        services.AddSingleton<ITextAnalyticsService, TextAnalyticsService>();

        return services;
    }

}
