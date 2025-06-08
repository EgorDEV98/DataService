using AutoMapper;
using DataService.Integration.Interfaces;
using DataService.Integration.Models;
using DataService.Integration.Tinkoff.Convertors;
using DataService.Integration.Tinkoff.Limiters;
using Google.Protobuf.WellKnownTypes;
using Tinkoff.InvestApi;
using Tinkoff.InvestApi.V1;
using CandleInterval = DataService.Integration.Enums.CandleInterval;

namespace DataService.Integration.Tinkoff.Provider;

/// <summary>
/// Реализация получения свечей
/// </summary>
public class CandleProvider(InvestApiClient client, MarketDataRateLimiter limiter, IMapper mapper) : ICandleProvider
{
    public async Task<IReadOnlyCollection<CandleDto>> GetCandlesAsync(string figi, CandleInterval interval, 
        DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken)
    {
        await limiter.WaitAsync(cancellationToken);
        var candles = (await client.MarketData.GetCandlesAsync(new GetCandlesRequest()
            {
                From = Timestamp.FromDateTimeOffset(from),
                To = Timestamp.FromDateTimeOffset(to),
                Interval = interval.Convert(),
                InstrumentId = figi,
                CandleSourceType = GetCandlesRequest.Types.CandleSource.IncludeWeekend,
            }, cancellationToken: cancellationToken)).Candles;
        return mapper.Map<IReadOnlyCollection<CandleDto>>(candles);
    }
}