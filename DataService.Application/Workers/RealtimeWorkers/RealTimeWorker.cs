using DataService.Application.Interfaces;
using DataService.Contracts.Models.Enums;
using DataService.Data;
using DataService.Data.Entities;
using DataService.Integration.Interfaces;
using DataService.Integration.Models.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

namespace DataService.Application.Workers.RealtimeWorkers;

[DisallowConcurrentExecution]
public class RealTimeWorker( 
    IServiceProvider serviceProvider, 
    ILogger<RealTimeWorker> logger, 
    ICandleBufferFlusher flusher) : IJob
{
    public static readonly JobKey Key = new(nameof(RealTimeWorker));
    
    private readonly Interval[] _intervals = [
        Interval._1Min,
        Interval._15Min
    ];
    
    public async Task Execute(IJobExecutionContext context)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken);
        try
        {
            logger.LogInformation("[{Worker}] Запуск", nameof(RealTimeWorker));
            await RunAsync(cts.Token);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "[{Worker}] Критическая ошибка: {Error}", nameof(RealTimeWorker), ex.Message);
            throw new JobExecutionException(ex, refireImmediately: true);
        }
    }
    
    private async Task RunAsync(CancellationToken ct)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<PostgresDbContext>();
        var candleStreamProvider = scope.ServiceProvider.GetRequiredService<ICandleStreamProvider>();
        
        var scheduler = await GetCurrentSessionAsync(ct);
        
        // Если на текущее время нет старта, тогда необходимо ожидать время, когда можно стартовать
        if (scheduler == null)
        {
            var nextDate = await db.Schedulers
                .AsNoTracking()
                .Where(x => x.IsTradingDay)
                .Where(x => x.StartTime >= DateTimeOffset.UtcNow)
                .FirstAsync(ct);
            
            var delayTime = GetDelayTime(nextDate.StartTime!.Value);
            await Task.Delay(delayTime, ct);
            return;
        }
        
        var shares = await db.Shares
            .AsNoTracking()
            .Where(s => s.CandleLoadStatus == LoadStatus.Enabled)
            .ToArrayAsync(ct);
        
        var figiMap = shares.ToDictionary(x => x.Figi, x => x.Id);

        candleStreamProvider.OnCandleReceived += async candle =>
        {
            if (figiMap.TryGetValue(candle.Figi, out var shareId))
            {
                var newCandle = new Candle
                {
                    Id = Guid.NewGuid(),
                    ShareId = shareId,
                    Open = candle.Open,
                    High = candle.High,
                    Low = candle.Low,
                    Close = candle.Close,
                    Volume = candle.Volume,
                    Time = candle.Time,
                    Interval = candle.Interval,
                    LoadType = LoadType.RealtimeWorker
                };
                flusher.Enqueue(newCandle);
            }
        };

        await flusher.StartAsync(ct);
        await candleStreamProvider.StartAsync(ct);

        foreach (var interval in _intervals)
        {
            await candleStreamProvider.Subscribe(
                shares.Select(s => new ExternalSubscribeShareRequest()
                {
                    Figi = s.Figi,
                    Interval = interval
                }), ct);
        }

        var delay = GetDelayTime(scheduler.EndTime!.Value);

        if (delay > TimeSpan.Zero)
        {
            logger.LogInformation("[{Worker}] Ожидание до конца сессии: {Delay}", nameof(RealTimeWorker), delay);
            await Task.Delay(delay, ct);
        }

        await candleStreamProvider.StopAsync(ct);
        await flusher.StopAsync();

        logger.LogInformation("[{Worker}] Завершён", nameof(RealTimeWorker));
    }
    
    private static TimeSpan GetDelayTime(DateTimeOffset time) => time - DateTimeOffset.UtcNow;
    private async Task<Scheduler?> GetCurrentSessionAsync(CancellationToken ct)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<PostgresDbContext>();
        var now = DateTimeOffset.UtcNow;
        return await db.Schedulers
            .AsNoTracking()
            .Where(x => x.IsTradingDay)
            .Where(x => x.StartTime <= now && x.EndTime >= now)
            .FirstOrDefaultAsync(ct);
    }
}