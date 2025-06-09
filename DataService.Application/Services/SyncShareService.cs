using AutoMapper;
using DataService.Application.Extensions;
using DataService.Application.Interfaces;
using DataService.Application.Options;
using DataService.Data;
using DataService.Data.Entities;
using DataService.Data.Extensions;
using DataService.Integration.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DataService.Application.Services;

/// <summary>
/// Реализация сервиса синхронизации акций
/// </summary>
public class SyncShareService(
    PostgresDbContext context,
    IOptions<SyncShareOptions> syncShareOptions,
    IShareProvider shareProvider,
    ILogger<SyncShareService> logger,
    IMapper mapper,
    IGuidProvider guidProvider)
    : ISyncShareService
{

    /// <summary>
    /// Синхронизировать акции
    /// </summary>
    /// <param name="ct"></param>
    /// <exception cref="NotImplementedException"></exception>
   public async Task SyncSharesAsync(CancellationToken ct = default)
    {
        var options = syncShareOptions.Value;
        logger.LogInformation("== Начата синхронизация акций ==");

        // Получаем все акции из БД (ключ = тикер)
        var dbShares = await context.Shares
            .ToDictionaryAsync(x => x.Ticker, ct);

        // Получаем акции от провайдера
        var providerShares = await shareProvider.GetSharesAsync(ct);

        var filteredShares = providerShares
            .WhereIf(options.IsQualInvestor.HasValue, x => x.ForQualInvestor == options.IsQualInvestor)
            .WhereIf(options.IncludesCurrency is { Length: > 0 }, x => options.IncludesCurrency!.Contains(x.Currency.ToUpper()))
            .WhereIf(options.IncludesClassCode is { Length: > 0 }, x => options.IncludesClassCode!.Contains(x.ClassCode.ToUpper()))
            .WhereIf(options.IncludesCountryCode is { Length: > 0 }, x => options.IncludesCountryCode!.Contains(x.CountryOfRisk.ToUpper()))
            .ToArray();

        // Если фильтр тикеров не задан — по умолчанию включить все
        var includesTickers = options.IncludesTicker is { Length: > 0 }
            ? options.IncludesTicker.ToHashSet(StringComparer.OrdinalIgnoreCase)
            : null;

        var newShares = new List<Share>();

        foreach (var providerShare in filteredShares)
        {
            var ticker = providerShare.Ticker;

            var shouldEnable = includesTickers is null || includesTickers.Contains(ticker);

            if (!dbShares.TryGetValue(ticker, out var existing))
            {
                // Новый тикер
                var newShare = mapper.Map<Share>(providerShare);
                newShare.Id = guidProvider.GetGuid();
                newShare.IsEnableToLoad = shouldEnable;
                newShares.Add(newShare);

                logger.LogInformation("Добавлен новый тикер: {Ticker}, Enabled: {Enabled}", ticker, shouldEnable);
            }
            else
            {
                // Обновление существующего тикера
                bool modified = false;

                if (existing.IsEnableToLoad != shouldEnable)
                {
                    existing.IsEnableToLoad = shouldEnable;
                    logger.LogInformation("Тикер {Ticker} изменил флаг IsEnableToLoad на {Value}", ticker, shouldEnable);
                    modified = true;
                }

                if (existing.Figi != providerShare.Figi)
                {
                    existing.Figi = providerShare.Figi;
                    modified = true;
                }

                if (existing.First1MinCandleDate != providerShare.First1MinCandleDate)
                {
                    existing.First1MinCandleDate = providerShare.First1MinCandleDate;
                    modified = true;
                }

                if (existing.First1DayCandleDate != providerShare.First1DayCandleDate)
                {
                    existing.First1DayCandleDate = providerShare.First1DayCandleDate;
                    modified = true;
                }

                if (modified)
                {
                    logger.LogDebug("Обновлены поля тикера: {Ticker}", ticker);
                }
            }
        }

        // Дополнительно — выключаем тикеры, удалённые из IncludesTicker
        if (includesTickers is not null)
        {
            var toDisable = dbShares.Values
                .Where(x => !includesTickers.Contains(x.Ticker) && x.IsEnableToLoad)
                .ToList();

            foreach (var item in toDisable)
            {
                item.IsEnableToLoad = false;
                logger.LogInformation("Тикер {Ticker} отключён (удалён из IncludesTicker)", item.Ticker);
            }
        }

        // Вставляем новые
        if (newShares.Count > 0)
        {
            await context.BulkInsertIgnoreDuplicatesAsync(newShares, cancellationToken: ct);
            logger.LogInformation("Добавлено новых акций: {Count}", newShares.Count);
        }

        // Сохраняем изменения
        await context.SaveChangesAsync(ct);
        logger.LogInformation("== Синхронизация акций завершена ==");   
    }   

}