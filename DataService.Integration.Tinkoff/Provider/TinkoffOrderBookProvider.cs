using AutoMapper;
using DataService.Integration.Interfaces;
using DataService.Integration.Models;
using DataService.Integration.Tinkoff.Limiters;
using Tinkoff.InvestApi;
using Tinkoff.InvestApi.V1;

namespace DataService.Integration.Tinkoff.Provider;

public class TinkoffOrderBookProvider(InvestApiClient client, MarketDataRateLimiter rateLimiter, IMapper mapper)
    : IOrderBookProvider
{
    public async Task<OrderBookDto> GetOrderBookAsync(string figi, int orderBookDepth, CancellationToken ct = default)
    {
        await rateLimiter.WaitAsync(ct);
        if(orderBookDepth > 50) orderBookDepth = 50;
        if(orderBookDepth < 0) orderBookDepth = 10;
        var orderBook = (await client.MarketData.GetOrderBookAsync(new GetOrderBookRequest()
        {
            Depth = orderBookDepth,
            InstrumentId = figi,
        }, cancellationToken: ct));
        
        return mapper.Map<OrderBookDto>(orderBook);
    }
}