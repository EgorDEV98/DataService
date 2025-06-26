using DataService.Integration.Interfaces;
using DataService.Integration.Models.Request;
using DataService.Integration.Models.Response;
using DataService.Integration.Tinkoff.Common;
using DataService.Integration.Tinkoff.Mappers;
using Google.Protobuf.WellKnownTypes;
using Tinkoff.InvestApi.V1;

namespace DataService.Integration.Tinkoff.Providers;

public class CandlesProvider(MarketDataService.MarketDataServiceClient client, MarketDataRateLimiter limiter, ExternalCandlesMapper mapper) : ICandlesProvider
{
    public async Task<IReadOnlyCollection<ExternalCandleResponse>> GetHistoryCandlesAsync(ExternalCandleRequest request, CancellationToken cancellationToken = default)
    {
        await limiter.WaitAsync(cancellationToken);
        var result = await client.GetCandlesAsync(new GetCandlesRequest()
        {
            Interval = mapper.Map(request.Interval),
            From = Timestamp.FromDateTimeOffset(request.From),
            To = Timestamp.FromDateTimeOffset(request.To),
            InstrumentId = request.Figi,
            CandleSourceType = GetCandlesRequest.Types.CandleSource.Exchange,
        }, cancellationToken: cancellationToken);
        var candles = result.Candles.ToList();
        return mapper.Map(candles);
    }
}