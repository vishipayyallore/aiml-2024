using AAI.TextAnalyticsApp.Interfaces;
using AAI.TextAnalyticsApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AAI.TextAnalyticsApp.Extensions;

public static class AAITextAnalyticsAppExtensions
{

    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration);

        services.AddKeyedScoped<ITextAnalyticsService, TextAnalyticsService>(nameof(TextAnalyticsService));

        services.AddKeyedScoped<ITextAnalyticsService, TextAnalyticsServiceRest>(nameof(TextAnalyticsServiceRest));

        return services;
    }

}
