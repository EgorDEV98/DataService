using DataService.Integration.Interfaces;
using DataService.Integration.Tinkoff.Limiters;
using DataService.Integration.Tinkoff.Options;
using DataService.Integration.Tinkoff.Provider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataService.Integration.Tinkoff;

/// <summary>
/// точка входа интеграции Тинькофф
/// </summary>
public static class Entry
{
    public static IServiceCollection AddTinkoffIntegration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTinkoffClient(configuration);
        services.AddProviders();

        services.AddAutoMapper(typeof(Entry).Assembly);
        services.AddSingleton<MarketDataRateLimiter>();
        
        
        return services;
    }

    private static void AddProviders(this IServiceCollection services)
    {
        services.AddScoped<IShareProvider, ShareProvider>();
    }

    private static void AddTinkoffClient(this IServiceCollection services, IConfiguration configuration)
    {
        var tinkoffOptions = configuration
            .GetSection(nameof(TinkoffOptions))
            .Get<TinkoffOptions>() ?? throw new NullReferenceException(nameof(TinkoffOptions));
        tinkoffOptions.Validate();

        services.AddInvestApiClient((_, options) =>
        {
            options.AccessToken = tinkoffOptions.AccessToken;
            options.Sandbox = tinkoffOptions.Sandbox;
            options.AppName = tinkoffOptions.AppName;
        });
    }
}