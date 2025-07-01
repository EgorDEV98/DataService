using DataService.Application.Interfaces;
using DataService.Application.Providers;
using DataService.Application.Services;
using DataService.Application.Workers;
using DataService.Application.Workers.HistoryWorkers;
using DataService.Application.Workers.RealtimeWorkers;
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
        services.AddSingleton<HandleHistoryWorker>();
        services.AddHostedService(sp => sp.GetRequiredService<HandleHistoryWorker>());
        
        services.AddQuartz(q =>
        {
            q.AddJob<NightHistoryWorker>(opts => opts.WithIdentity(NightHistoryWorker.JobKey));
            
            // ===== Cron триггер
            q.AddTrigger(opts => opts
                .ForJob(NightHistoryWorker.JobKey)
                .WithIdentity($"{nameof(NightHistoryWorker)}-cron-trigger")
                .WithCronSchedule("0 1 0 * * ?"));
            
            // ===== Стартап триггер
            q.AddTrigger(opts => opts
                .ForJob(NightHistoryWorker.JobKey)
                .WithIdentity($"{nameof(NightHistoryWorker)}-on-startup-trigger")
                .StartNow());
            
            // === OneMinuteRealtimeCandleWorker
            q.AddJob<RealTimeWorker>(opts => opts.WithIdentity(RealTimeWorker.Key));
            
            q.AddTrigger(opts => opts
                .ForJob(RealTimeWorker.Key)
                .WithIdentity($"{nameof(RealTimeWorker)}-scheduled-trigger")
                .WithSimpleSchedule(x =>
                {
                    x.WithIntervalInSeconds(1);
                    x.RepeatForever();
                }));

           
        });
        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
    }

    private static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IShareService, SharesService>();
        services.AddScoped<IOrderBookService, OrderBookService>();
        services.AddScoped<ISchedulersService, SchedulersService>();
        services.AddScoped<ICandlesService, CandlesService>();
        services.AddSingleton<ICandleBufferFlusher, CandleBufferFlusher>();
    }

    private static void AddProviders(this IServiceCollection services)
    {
        services.AddSingleton<IGuidProvider, GuidProvider>();
    }
}