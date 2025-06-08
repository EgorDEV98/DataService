using DataService.Application.Interfaces;
using DataService.Application.Options;
using DataService.Application.Provider;
using DataService.Application.Services;
using DataService.Application.Workers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace DataService.Application;

public static class Entry
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCustomOptions(configuration);
        services.AddSingleton<IGuidProvider, GuidProvider>();
        services.AddScoped<ISyncShareService, SyncShareService>();
        services.AddAutoMapper(typeof(Entry).Assembly);
        services.AddWorkers(configuration);
        
        return services;
    }

    private static IServiceCollection AddWorkers(this IServiceCollection services, IConfiguration configuration)
    {
        var cronOptions = configuration
            .GetSection(nameof(CronOptions))
            .Get<CronOptions>() ?? throw new NullReferenceException(nameof(CronOptions));
        
        services.AddQuartz(q =>
        {
            var historyCandleWorkerJobKey = new JobKey(nameof(SyncHistoryCandlesWorker));
            q.AddJob<SyncHistoryCandlesWorker>(opts => opts.WithIdentity(historyCandleWorkerJobKey));
            
            // ===== Cron триггер
            q.AddTrigger(opts => opts
                .ForJob(historyCandleWorkerJobKey)
                .WithIdentity($"{nameof(SyncHistoryCandlesWorker)}-cron-trigger")
                .WithCronSchedule(cronOptions.SyncHistoryCandlesCron));
            
            // ===== Стартап триггер
            q.AddTrigger(opts => opts
                .ForJob(historyCandleWorkerJobKey)
                .WithIdentity($"{nameof(SyncHistoryCandlesWorker)}-on-startup-trigger")
                .StartNow());
        });
        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

        return services;
    }

    private static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
    {
        var syncShareOptions = configuration.GetSection(nameof(SyncShareOptions));
        services.Configure<SyncShareOptions>(syncShareOptions.Bind);

        return services;
    }
}