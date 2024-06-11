using HeaderFooter;
using HeaderFooter.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace cv_detect_people.Extensions;

public static class ReadTextAppExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IHeader, Header>();

        services.AddScoped<IFooter, Footer>();

        return services;
    }
}
