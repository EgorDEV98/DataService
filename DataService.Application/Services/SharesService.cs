using DataService.Application.Interfaces;
using DataService.Application.Models;
using DataService.Application.Workers;
using DataService.Contracts.Models.Enums;
using DataService.Data;
using DataService.Data.Entities;
using DataService.Data.Extensions;
using DataService.Integration.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataService.Application.Services;

public class SharesService(PostgresDbContext context, IGuidProvider guidProvider, ISharesProvider sharesProvider, HistoryCandleLoadWorker historyCandleLoadWorker) : IShareService
{
    public async Task<Share> GetShareAsync(GetShareParams param, CancellationToken cancellationToken = default)
    {
        var share = await context.Shares
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == param.Id, cancellationToken);
        if(share == null) throw new KeyNotFoundException("Share not found");
        return share;
    }

    public async Task<IReadOnlyCollection<Share>> GetSharesAsync(GetSharesParams param, CancellationToken cancellationToken = default)
        => await context.Shares
            .AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(param.Ticker), x => x.Ticker == param.Ticker)
            .WhereIf(!string.IsNullOrWhiteSpace(param.Figi), x => x.Figi == param.Figi)
            .WhereIf(!string.IsNullOrWhiteSpace(param.ClassCode), x => x.ClassCode == param.ClassCode)
            .WhereIf(!string.IsNullOrWhiteSpace(param.Currency), x => x.Currency == param.Currency)
            .WhereIf(param.CandleLoadStatus.HasValue, x => x.CandleLoadStatus == param.CandleLoadStatus)
            .Skip(param.Offset ?? 0)
            .Take(param.Limit ?? 100)
            .ToArrayAsync(cancellationToken);

    public async Task<bool> ChangeLoadStatus(ChangeLoadStatusParams param, CancellationToken cancellationToken = default)
    {
        var share = await context.Shares.FirstOrDefaultAsync(x => x.Id == param.Id, cancellationToken);
        if(share == null) throw new KeyNotFoundException("Share not found");
        
        share.CandleLoadStatus = param.CandleLoadStatus;
        await context.SaveChangesAsync(cancellationToken);
        if (share.CandleLoadStatus == LoadStatus.Enabled)
            await historyCandleLoadWorker.EnqueueAsync(param.Id);
        
        return true;
    }

    public async Task<bool> PreloadSharesAsync(CancellationToken cancellationToken)
    {
        var externalShares = (await sharesProvider.GetSharesAsync(cancellationToken))
            .Select(x => new Share()
            {
                Id = guidProvider.GetGuid(),
                Currency = x.Currency,
                ClassCode = x.ClassCode,
                Figi = x.Figi,
                CandleLoadStatus = LoadStatus.Disabled,
                WeekendFlag = x.WeekendFlag,
                CountryOfRisk = x.CountryOfRisk,
                DivYieldFlag = x.DivYieldFlag,
                MinPriceIncrement = x.MinPriceIncrement,
                ShortEnabledFlag = x.ShortEnabledFlag,
                CountryOfRiskName = x.CountryOfRiskName,
                First1DayCandleDate = x.First1DayCandleDate,
                First1MinCandleDate = x.First1MinCandleDate,
                Ticker = x.Ticker,
                Lot = x.Lot,
                Name = x.Name,
                Sector = x.Sector,
            })
            .ToArray();
        await context.BulkInsertAsync(externalShares, cancellationToken: cancellationToken);
        return true;
    }
}