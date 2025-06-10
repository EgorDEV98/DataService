using DataService.Application.Interfaces;
using DataService.Application.Mq.Publisher;
using DataService.Application.Options;
using DataService.Contracts.Models.Enums;
using DataService.Contracts.Models.Mq;
using DataService.Data;
using DataService.Data.Entities;
using DataService.Integration.Interfaces;
using DataService.Integration.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

namespace DataService.Application.Workers.RealTimeWorkers;

public abstract class RealTimeCandleWorkerBase<TWorker>(
    IServiceProvider serviceProvider,
    ILogger<TWorker> logger,
    IOptions<CronOptions> cronOptions,
    IOptions<SessionOptions> sessionOptions,
    ICandleBufferFlusher flusher,
    Interval interval,
    Interval subscribeInterval)
    where TWorker : class
{
    public async Task Execute(IJobExecutionContext context)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken);
        try
        {
            logger.LogInformation("[{Worker}] Запуск", typeof(TWorker).Name);
            await RunAsync(cts.Token);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "[{Worker}] Критическая ошибка", typeof(TWorker).Name);

            if (IsWithinSession())
                throw new JobExecutionException(ex, refireImmediately: true);

            throw;
        }
    }

    private async Task RunAsync(CancellationToken ct)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<PostgresDbContext>();
        var stream = scope.ServiceProvider.GetRequiredService<ICandleStreamProvider>();
        var publisher = scope.ServiceProvider.GetRequiredService<CandlePublisher>();

        var shares = await db.Shares.AsNoTracking().Where(s => s.IsEnableToLoad).ToListAsync(ct);
        var figiMap = shares.ToDictionary(x => x.Figi, x => x.Id);

        stream.OnCandleReceived += async candle =>
        {
            if (figiMap.TryGetValue(candle.Figi, out var shareId))
            {
                flusher.Enqueue(new Candle
                {
                    Id = Guid.NewGuid(),
                    ShareId = shareId,
                    Open = candle.Open,
                    High = candle.High,
                    Low = candle.Low,
                    Close = candle.Close,
                    Volume = candle.Volume,
                    Time = candle.Time,
                    LoadType = LoadType.RealtimeSync,
                    Interval = interval
                });
                
                await publisher.PublishAsync(new NewCandle(
                    candle.Figi,
                    candle.Time,
                    interval,
                    candle.Open,
                    candle.High,
                    candle.Low,
                    candle.Close,
                    candle.Volume), ct);
            }
        };

        await flusher.StartAsync(ct);
        await stream.StartAsync(ct);

        await stream.Subscribe(
            shares.Select(s => new SubscribeShareDto
            {
                Figi = s.Figi,
                Interval = subscribeInterval
            }),
            ct);

        var end = DateTimeOffset.UtcNow.Date.Add(sessionOptions.Value.SessionEndTime);
        var delay = end - DateTimeOffset.UtcNow;

        if (delay > TimeSpan.Zero)
        {
            logger.LogInformation("[{Worker}] Ожидание до конца сессии: {Delay}", typeof(TWorker).Name, delay);
            await Task.Delay(delay, ct);
        }

        await stream.StopAsync(ct);
        await flusher.StopAsync();

        logger.LogInformation("[{Worker}] Завершён", typeof(TWorker).Name);
    }

    private bool IsWithinSession()
    {
        var now = DateTimeOffset.UtcNow.TimeOfDay;
        var start = sessionOptions.Value.SessionStartTime;
        var end = sessionOptions.Value.SessionEndTime;
        return now >= start && now <= end;
    }
}