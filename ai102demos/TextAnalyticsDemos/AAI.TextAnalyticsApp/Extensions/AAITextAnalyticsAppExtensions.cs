﻿using AAI.TextAnalyticsApp.Interfaces;
using AAI.TextAnalyticsApp.Services;
using HeaderFooter;
using HeaderFooter.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AAI.TextAnalyticsApp.Extensions;

public static class AAITextAnalyticsAppExtensions
{

    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddHttpClient();

        services.AddScoped<ITextAnalyticsRequestBuilder, TextAnalyticsRequestBuilder>();

        services.AddKeyedScoped<ITextAnalyticsService, TextAnalyticsService>(nameof(TextAnalyticsService));

        services.AddKeyedScoped<ITextAnalyticsService, TextAnalyticsServiceRest>(nameof(TextAnalyticsServiceRest));

        services.AddScoped<IHeader, Header>();

        services.AddScoped<IFooter, Footer>();

        return services;
    }

}
