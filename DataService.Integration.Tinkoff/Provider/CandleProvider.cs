using AutoMapper;
using DataService.Contracts.Models.Enums;
using DataService.Integration.Interfaces;
using DataService.Integration.Models;
using DataService.Integration.Tinkoff.Limiters;
using Google.Protobuf.WellKnownTypes;
using Tinkoff.InvestApi;
using Tinkoff.InvestApi.V1;

namespace DataService.Integration.Tinkoff.Provider;

/// <summary>
/// Реализация получения свечей
/// </summary>
public class CandleProvider(InvestApiClient client, MarketDataRateLimiter limiter, IMapper mapper) : ICandleProvider
{
    public async Task<IReadOnlyCollection<CandleDto>> GetCandlesAsync(string figi, Interval interval, 
        DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken)
    {
        await limiter.WaitAsync(cancellationToken);
        var candles = (await client.MarketData.GetCandlesAsync(new GetCandlesRequest()
            {
                From = Timestamp.FromDateTimeOffset(from),
                To = Timestamp.FromDateTimeOffset(to),
                Interval = Convert(interval),
                InstrumentId = figi,
                CandleSourceType = GetCandlesRequest.Types.CandleSource.IncludeWeekend,
            }, cancellationToken: cancellationToken)).Candles;
        return mapper.Map<IReadOnlyCollection<CandleDto>>(candles);
    }

    private CandleInterval Convert(Interval interval)
        => interval switch
        {
            Interval._1Min => CandleInterval._1Min,
            Interval._15Min => CandleInterval._15Min,
            _ => throw new ArgumentOutOfRangeException(nameof(interval), interval, null)
        };
}