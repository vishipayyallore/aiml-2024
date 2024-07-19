using HeaderFooter;
using HeaderFooter.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FirstSample.Extensions;

public static class AppExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IHeader, Header>();

        services.AddScoped<IFooter, Footer>();

        return services;
    }
}
