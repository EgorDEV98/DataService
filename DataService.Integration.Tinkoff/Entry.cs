using DataService.Integration.Interfaces;
using DataService.Integration.Tinkoff.Common;
using DataService.Integration.Tinkoff.Mappers;
using DataService.Integration.Tinkoff.Options;
using DataService.Integration.Tinkoff.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tinkoff.InvestApi;
using Tinkoff.InvestApi.V1;

namespace DataService.Integration.Tinkoff;

public static class Entry
{
    public static IServiceCollection AddTinkoff(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddClient(configuration);
        services.AddRateLimiters();
        services.AddProviders();
        services.AddMappers();
        
        return services;
    }

    private static void AddMappers(this IServiceCollection services)
    {
        services.AddSingleton<ExternalShareMapper>();
        services.AddSingleton<ExternalOrderBookMapper>();
    }
    private static void AddProviders(this IServiceCollection services)
    {
        services.AddScoped<ISharesProvider, SharesProvider>();
        services.AddScoped<IOrderBookProvider, OrderBookProvider>();
    }

    private static void AddRateLimiters(this IServiceCollection services)
    {
        services.AddSingleton<InstrumentRateLimiter>();
        services.AddSingleton<MarketDataRateLimiter>();
    }
    
    private static void AddClient(this IServiceCollection services, IConfiguration configuration)
    {
        var tinkoffOptions = configuration
            .GetSection(nameof(TinkoffApiOptions))
            .Get<TinkoffApiOptions>() ?? throw new NullReferenceException(nameof(TinkoffApiOptions) + " is null.");
        tinkoffOptions.Validate();

        services.AddInvestApiClient((_, options) =>
        {
            options.AccessToken = tinkoffOptions.AccessToken;
            options.AppName = tinkoffOptions.AppName;
            options.Sandbox = tinkoffOptions.Sandbox;
        });
        services.AddTinkoffClients();
    }

    private static void AddTinkoffClients(this IServiceCollection services)
    {
        services.AddScoped(sp => sp.GetRequiredService<InvestApiClient>().Instruments);
        services.AddScoped(sp => sp.GetRequiredService<InvestApiClient>().MarketData);
    }
}