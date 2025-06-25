using DataService.Integration.Interfaces;
using DataService.Integration.Models.Request;
using DataService.Integration.Models.Response;
using DataService.Integration.Tinkoff.Common;
using DataService.Integration.Tinkoff.Mappers;
using Tinkoff.InvestApi.V1;

namespace DataService.Integration.Tinkoff.Providers;

public class OrderBookProvider(MarketDataService.MarketDataServiceClient client, MarketDataRateLimiter limiter, ExternalOrderBookMapper mapper) : IOrderBookProvider
{
    public async Task<ExternalGetOrderBookResponse> GetOrderBookAsync(ExternalGetOrderBookRequest request, CancellationToken cancellationToken = default)
    {
        await limiter.WaitAsync(cancellationToken);
        var orderBookResponse = await client.GetOrderBookAsync(new GetOrderBookRequest()
        {
            InstrumentId = request.Figi,
            Depth = request.Depth,
        }, cancellationToken: cancellationToken);
        
        if (orderBookResponse == null)
            throw new InvalidOperationException("Received null OrderBook response from gRPC");
        return mapper.Map(orderBookResponse);
    }
}