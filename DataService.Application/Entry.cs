using DataService.Application.Interfaces;
using DataService.Application.Mq.Publisher;
using DataService.Application.Options;
using DataService.Application.Provider;
using DataService.Application.Services;
using DataService.Application.Workers;
using DataService.Application.Workers.RealTimeWorkers;
using DataService.Contracts.Models.Mq;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace DataService.Application;

public static class Entry
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCustomOptions(configuration);
        services.AddWorkers(configuration);
        services.AddServices();
        services.AddRabbit(configuration);
        
        services.AddAutoMapper(typeof(Entry).Assembly);
        return services;
    }

    private static IServiceCollection AddRabbit(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<CandlePublisher>();
        services.AddScoped<ShareSyncedNotificatorPublisher>();
        
        var mqOptions = configuration
            .GetSection(nameof(RabbitMqOptions))
            .Get<RabbitMqOptions>() ?? throw new NullReferenceException(nameof(RabbitMqOptions));

        services.AddMassTransit(cfg =>
        {
            cfg.UsingRabbitMq((ctx, bus) =>
            {
                bus.Host(mqOptions.Host, mqOptions.VirtualHost, h =>
                {
                    h.Username(mqOptions.Username);
                    h.Password(mqOptions.Password);
                });

                bus.ConfigureEndpoints(ctx);
                
                bus.Message<NewCandle>(x => { x.SetEntityName("candles"); });
                bus.Publish<NewCandle>(x => { x.ExchangeType = "fanout"; });
                
                bus.Message<ShareSynced>(x => { x.SetEntityName("shares.synced"); });
                bus.Publish<ShareSynced>(x => { x.ExchangeType = "fanout"; });
                
                bus.ReceiveEndpoint("candles-expiring", e =>
                {
                    e.ConfigureConsumeTopology = false;
                    e.SetQueueArgument("x-message-ttl", 60000);
                    e.Bind("candles");
                });
                
                bus.ConfigureEndpoints(ctx);
            });
        });
        
        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<CandlesService>();
        services.AddScoped<SharesService>();
        services.AddScoped<ISyncShareService, SyncShareService>();
        services.AddSingleton<ICandleBufferFlusher, CandleBufferFlusher>();
        services.AddSingleton<IGuidProvider, GuidProvider>();
        services.AddScoped<OrderBookService>();

        return services;
    }

    private static IServiceCollection AddWorkers(this IServiceCollection services, IConfiguration configuration)
    {
        var cronOptions = configuration
            .GetSection(nameof(CronOptions))
            .Get<CronOptions>() ?? throw new NullReferenceException(nameof(CronOptions));
        
        var sessionOptions = configuration
            .GetSection(nameof(SessionOptions))
            .Get<SessionOptions>() ?? throw new NullReferenceException(nameof(SessionOptions));
        
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
            
            var sessionStart = ParseTimeUtc(sessionOptions.SessionStartTime);
            var cronExpression = $"{sessionStart.Second} {sessionStart.Minute} {sessionStart.Hour} ? * *";
            
            // === OneMinuteRealtimeCandleWorker
            var realtimeKey = new JobKey(nameof(OneMinuteRealtimeCandleWorker));
            q.AddJob<OneMinuteRealtimeCandleWorker>(opts => opts.WithIdentity(realtimeKey));
            
            q.AddTrigger(opts => opts
                .ForJob(realtimeKey)
                .WithIdentity($"{nameof(OneMinuteRealtimeCandleWorker)}-scheduled-trigger")
                .WithCronSchedule(cronExpression, x => x.WithMisfireHandlingInstructionFireAndProceed()));

            q.AddTrigger(opts => opts
                .ForJob(realtimeKey)
                .WithIdentity($"{nameof(OneMinuteRealtimeCandleWorker)}-startup-trigger")
                .StartNow());
            
            var realtime15Key = new JobKey(nameof(FifteenMinuteRealtimeCandleWorker));
            q.AddJob<FifteenMinuteRealtimeCandleWorker>(opts => opts.WithIdentity(realtime15Key));
            
            q.AddTrigger(opts => opts
                .ForJob(realtime15Key)
                .WithIdentity($"{nameof(FifteenMinuteRealtimeCandleWorker)}-scheduled-trigger")
                .WithCronSchedule(cronExpression, x => x.WithMisfireHandlingInstructionFireAndProceed()));
            
            q.AddTrigger(opts => opts
                .ForJob(realtime15Key)
                .WithIdentity($"{nameof(FifteenMinuteRealtimeCandleWorker)}-startup-trigger")
                .StartNow());
        });
        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        
       
        return services;
    }

    private static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
    {
        var syncShareOptions = configuration.GetSection(nameof(SyncShareOptions));
        services.Configure<SyncShareOptions>(syncShareOptions.Bind);
        
        var cronOptions = configuration.GetSection(nameof(CronOptions));
        services.Configure<CronOptions>(cronOptions.Bind);

        var sessionOptions = configuration.GetSection(nameof(SessionOptions));
        services.Configure<SessionOptions>(sessionOptions.Bind);

        var rabbitMqOptions = configuration.GetSection(nameof(RabbitMqOptions));
        services.Configure<RabbitMqOptions>(rabbitMqOptions.Bind);
        
        return services;
    }
    
    private static DateTimeOffset ParseTimeUtc(TimeSpan time)
    {
        var today = DateTime.UtcNow.Date;
        var target = today.Add(time);
        return DateTime.SpecifyKind(target, DateTimeKind.Utc);
    }
}