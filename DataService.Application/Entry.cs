using DataService.Application.Interfaces;
using DataService.Application.Providers;
using DataService.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataService.Application;

public static class Entry
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddProviders();
        services.AddServices();
        
        return services;
    }

    private static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IShareService, SharesService>();
        services.AddScoped<IOrderBookService, OrderBookService>();
        services.AddScoped<ISchedulersService, SchedulersService>();
        services.AddScoped<ICandlesService, CandlesService>();
    }

    private static void AddProviders(this IServiceCollection services)
    {
        services.AddSingleton<IGuidProvider, GuidProvider>();
    }
}