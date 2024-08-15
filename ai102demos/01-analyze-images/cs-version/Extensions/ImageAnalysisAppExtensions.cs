using HeaderFooter;
using HeaderFooter.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ImageAnalysis.Extensions;

public static class ImageAnalysisAppExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IHeader, Header>();

        services.AddScoped<IFooter, Footer>();

        return services;
    }
}
