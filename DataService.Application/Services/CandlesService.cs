using DataService.Application.Interfaces;
using DataService.Application.Models;
using DataService.Data;
using DataService.Data.Entities;
using DataService.Data.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DataService.Application.Services;

public class CandlesService(PostgresDbContext context) : ICandlesService
{
    public async Task<IReadOnlyCollection<Candle>> GetCandlesAsync(GetCandlesParams param,
        CancellationToken cancellationToken = default)
        => await context.Candles
            .Where(x => x.ShareId == param.ShareId)
            .Where(x => x.Interval == param.Interval)
            .WhereIf(param.From.HasValue, x => x.Time >= param.From)
            .WhereIf(param.To.HasValue, x => x.Time <= param.To)
            .OrderByDescending(x => x.Time)
            .Skip(param.Offset ?? 0)
            .Take(param.Limit ?? 100)
            .ToArrayAsync(cancellationToken);
}