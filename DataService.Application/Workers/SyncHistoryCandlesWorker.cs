using DataService.Application.Interfaces;
using DataService.Data;
using DataService.Data.Entities;
using DataService.Data.Enum;
using DataService.Data.Extensions;
using DataService.Integration.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

namespace DataService.Application.Workers;

/// <summary>
/// Сервис синхронизации исторических свечей
/// </summary>
public class SyncHistoryCandlesWorker(
    IServiceProvider serviceProvider, 
    ILogger<SyncHistoryCandlesWorker> logger,
    IGuidProvider guidProvider) : IJob
{

    private readonly CandleInterval[] IntervalsForLoad = [
        CandleInterval._1Min,
        CandleInterval._15Min
    ];
    
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("[{WorkerName}] Начал работу", nameof(SyncHistoryCandlesWorker));
        await Run(context.CancellationToken);
        logger.LogInformation("[{WorkerName}] Завершил работу", nameof(SyncHistoryCandlesWorker));
    }

    private async Task Run(CancellationToken ct)
    {
        // Получаем зависимости
        await using var scope = serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<PostgresDbContext>();
        var candleProvider = scope.ServiceProvider.GetRequiredService<ICandleProvider>();

        // Список акций включенных в загрузку
        var shares = await context.Shares
            .AsNoTracking()
            .Where(x => x.IsEnableToLoad)
            .ToArrayAsync(ct);
        logger.LogInformation("[{WorkerName}] К загрузке {ShareCount} акций", nameof(SyncHistoryCandlesWorker), shares.Length);

        // Проходимся по всем акциям
        foreach (var share in shares)
            await LoadCandle(context, share, candleProvider, ct);
    }

    private async Task LoadCandle(PostgresDbContext context, Share share, ICandleProvider candleProvider, CancellationToken ct)
    {
        foreach (var interval in IntervalsForLoad)
        {
            // Получаем последнюю загруженную свечу будет являться FROM датой
            // В зависимости от интервала устанавливаем точку TO
            var from = await GetLastLoadCandle(context, share,interval, ct);

            while (from <= DateTimeOffset.UtcNow && !ct.IsCancellationRequested)
            {
                await LoadCandleByInterval(context, share, interval, from, candleProvider, ct);

                from = GetToDate(interval, from);
            }
        }
    }

    private async Task LoadCandleByInterval(PostgresDbContext context, Share share, CandleInterval interval, DateTimeOffset from, ICandleProvider candleProvider, CancellationToken ct)
    {
        // Из интервала определяем дату TO
        var toDate = GetToDate(interval, from);
        var candles = (await candleProvider.GetCandlesAsync(share.Figi, Convert(interval), from, toDate, ct))
            .Select(x => new Candle()
            {
                Close = x.Close,
                High = x.High,
                Low = x.Low,
                Open = x.Open,
                Time = x.Time,
                Interval = interval,
                Volume = x.Volume,
                ShareId = share.Id,
                LoadType = LoadType.NightSync,
                Id = guidProvider.GetGuid()
            }).ToArray();
        await context.BulkInsertIgnoreDuplicatesAsync(candles, cancellationToken: ct);
        logger.LogInformation("[{WorkerName}] {Ticker} Загружено {CandlesCount} свечей с интервалом {Interval}",
            nameof(SyncHistoryCandlesWorker), 
            share.Ticker,
            candles.Length,
            interval);

    }

    private DataService.Integration.Enums.CandleInterval Convert(CandleInterval interval)
        => interval switch
        {
            CandleInterval._1Min => DataService.Integration.Enums.CandleInterval._1Min,
            CandleInterval._15Min => DataService.Integration.Enums.CandleInterval._15Min,
            _ => throw new ArgumentOutOfRangeException(nameof(interval), interval, null)
        };
    private static DateTimeOffset GetToDate(CandleInterval interval, DateTimeOffset from)
        => interval switch
        {
            CandleInterval._1Min => from.AddDays(1),
            CandleInterval._15Min => from.AddDays(21),
            _ => throw new ArgumentOutOfRangeException()
        };

    private async Task<DateTimeOffset> GetLastLoadCandle(PostgresDbContext context, Share share,
        CandleInterval interval, CancellationToken ct)
    {
        // Получаем последнюю загруженную свечу учитывая интервал
        var lastLoadCandle = await context.Candles
            .AsNoTracking()
            .Where(x => x.ShareId == share.Id)
            .Where(x => x.Interval == interval)
            .MaxAsync(x => (DateTimeOffset?)x.Time, ct);
        
        // Определяем с какого периода будем собирать свечи
        // Приоритет:
        // 1. Последняя загруженная свеча
        // 2. Год назад
        // 3. Если свечи меньше чем год назад, тогда первую свечу 
        var oneHalfYearAgoDate = DateTime.UtcNow.AddMonths(-1).Date;
        var firstCandleDate = share.First1MinCandleDate.Date;
        return lastLoadCandle?.UtcDateTime.Date 
                       ?? (firstCandleDate > oneHalfYearAgoDate ? firstCandleDate : oneHalfYearAgoDate);
    }
}