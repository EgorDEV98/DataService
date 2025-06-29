using DataService.Application.Interfaces;
using DataService.Application.Providers;
using DataService.Application.Services;
using DataService.Application.Workers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace DataService.Application;

public static class Entry
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddProviders();
        services.AddServices();
        services.AddBackgroundWorkers();
        
        return services;
    }

    private static void AddBackgroundWorkers(this IServiceCollection services)
    {
        services.AddSingleton<HistoryCandleLoadWorker>();
        services.AddHostedService(sp => sp.GetRequiredService<HistoryCandleLoadWorker>());
        
        services.AddQuartz(q =>
        {
            q.AddJob<NightSyncWorker>(opts => opts.WithIdentity(NightSyncWorker.JobKey));
            
            // ===== Cron триггер
            q.AddTrigger(opts => opts
                .ForJob(NightSyncWorker.JobKey)
                .WithIdentity($"{nameof(NightSyncWorker)}-cron-trigger")
                .WithCronSchedule("0 1 0 * * ?"));
            
            // ===== Стартап триггер
            q.AddTrigger(opts => opts
                .ForJob(NightSyncWorker.JobKey)
                .WithIdentity($"{nameof(NightSyncWorker)}-on-startup-trigger")
                .StartNow());
        });
        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
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