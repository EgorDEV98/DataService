using DataService.Application.Interfaces;
using DataService.Application.Options;
using DataService.Application.Provider;
using DataService.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataService.Application;

public static class Entry
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCustomOptions(configuration);
        services.AddSingleton<IGuidProvider, GuidProvider>();
        services.AddScoped<ISyncShareService, SyncShareService>();
        services.AddAutoMapper(typeof(Entry).Assembly);
        
        return services;
    }

    private static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
    {
        var syncShareOptions = configuration.GetSection(nameof(SyncShareOptions));
        services.Configure<SyncShareOptions>(syncShareOptions.Bind);

        return services;
    }
}