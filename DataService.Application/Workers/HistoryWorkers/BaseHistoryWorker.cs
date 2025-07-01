using DataService.Application.Interfaces;
using DataService.Contracts.Models.Enums;
using DataService.Data;
using DataService.Data.Entities;
using DataService.Data.Extensions;
using DataService.Integration.Interfaces;
using DataService.Integration.Models.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataService.Application.Workers.HistoryWorkers;

public class BaseHistoryWorker<T>(ILogger<T> logger, IGuidProvider guidProvider)
{
    private readonly Interval[] _intervals =
    [
        Interval._1Min,
        Interval._15Min
    ];
    
    protected async Task StartWork(Share share, PostgresDbContext context, ICandlesProvider candleProvider, CancellationToken ct)
    {
        foreach (var interval in _intervals)
            await LoadCandlesForInterval(share, context, interval, candleProvider, ct);
    }
    private async Task LoadCandlesForInterval(Share share, PostgresDbContext context, Interval interval,
        ICandlesProvider candleProvider, CancellationToken ct)
    {
        var fromDate = await GetLastLoadCandle(context, share, interval, ct);
        var toDate = GetToDate(interval, fromDate);

        while (fromDate <= DateTimeOffset.UtcNow && !ct.IsCancellationRequested)
        {
            try
            {
                var candles = (await candleProvider.GetHistoryCandlesAsync(new ExternalCandleRequest()
                {
                    Figi = share.Figi,
                    Interval = interval,
                    From = fromDate,
                    To = toDate,
                }, ct))
                .Select(c => new Candle()
                {
                    Id = guidProvider.GetGuid(),
                    Interval = interval,
                    Time = c.Time,
                    Close = c.Close,
                    Open = c.Open,
                    High = c.High,
                    Low = c.Low,
                    Volume = c.Volume,
                    LoadType = LoadType.HistoryWorker,
                    ShareId = share.Id,
                });
                await context.BulkInsertAsync(candles, cancellationToken: ct);

                fromDate = toDate;
                toDate = GetToDate(interval, fromDate);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при загрузке свечей {Ticker}, {Interval}", share.Ticker, interval);
            }
        }
    }
    
    private static DateTimeOffset GetToDate(Interval interval, DateTimeOffset from)
        => interval switch
        {
            Interval._1Min => from.AddDays(1),
            Interval._2Min => from.AddDays(1),
            Interval._3Min => from.AddDays(1),
            Interval._5Min => from.AddDays(7),
            Interval._10Min => from.AddDays(7),
            Interval._15Min => from.AddDays(21),
            Interval._1Hour => from.AddMonths(3),
            Interval._2Hour => from.AddMonths(3),
            Interval._4Hour => from.AddMonths(3),
            Interval._1Day => from.AddYears(6),
            Interval._1Week => from.AddYears(5),
            Interval._1Month => from.AddYears(10),
            _ => throw new ArgumentOutOfRangeException()
        };

    private async Task<DateTimeOffset> GetLastLoadCandle(PostgresDbContext context, Share share, Interval interval,
        CancellationToken ct)
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
        // 2. Пол года назад (на случай если нужен будет бектест)
        // 3. Если свечи меньше чем год назад, тогда первую свечу 
        var oneHalfYearAgoDate = DateTime.UtcNow.AddMonths(-6).Date;
        var firstCandleDate = share.First1MinCandleDate;
        return lastLoadCandle ?? (firstCandleDate > oneHalfYearAgoDate ? firstCandleDate : oneHalfYearAgoDate);
    }
}