using AutoMapper;
using DataService.Application.Models;
using DataService.Contracts.Models.Response;
using DataService.Data;
using DataService.Data.Enum;
using DataService.Data.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DataService.Application.Services;

public class CandlesService(PostgresDbContext context, IMapper mapper)
{
    public async Task<IReadOnlyCollection<CandleResponse>> GetCandlesAsync(GetCandlesParams param, CancellationToken ct)
    {
        var candles = await context.Candles
            .AsNoTracking()
            .Where(x => x.ShareId == param.ShareId)
            .Where(x => x.Interval == ConvertInterval(param.Interval))
            .WhereIf(param.OnlyMainSession.HasValue, x => x.Time.Hour >= 10 && (x.Time.Hour <= 18 && x.Time.Hour <= 40))
            .WhereIf(param.ExcludeWeekend.HasValue, x => x.Time.DayOfWeek != DayOfWeek.Saturday || x.Time.DayOfWeek != DayOfWeek.Sunday)
            .WhereIf(param.From.HasValue, x => x.Time >= param.From)
            .WhereIf(param.To.HasValue, x => x.Time <= param.To)
            .OrderByDescending(x => x.Time)
            .Skip(param.Offset ?? 0)
            .Take(param.Limit ?? 100)
            .ToArrayAsync(ct);
        return mapper.Map<IReadOnlyCollection<CandleResponse>>(candles);
    }

    private CandleInterval ConvertInterval(Contracts.Models.Enums.CandleInterval interval)
        => interval switch
        {
            Contracts.Models.Enums.CandleInterval._1Min => CandleInterval._1Min,
            Contracts.Models.Enums.CandleInterval._15Min => CandleInterval._15Min,
        };
}