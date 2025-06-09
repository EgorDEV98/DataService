using DataService.Integration.Interfaces;
using DataService.Integration.Models;
using DataService.Integration.Tinkoff.Limiters;
using Tinkoff.InvestApi;
using Tinkoff.InvestApi.V1;

namespace DataService.Integration.Tinkoff.Provider;

public class TinkoffOrderBookProvider : IOrderBookProvider
{
    private readonly InvestApiClient _client;
    private readonly MarketDataRateLimiter _rateLimiter;

    public TinkoffOrderBookProvider(InvestApiClient client, MarketDataRateLimiter rateLimiter)
    {
        _client = client;
        _rateLimiter = rateLimiter;
    }
    
    public async Task<OrderBookDto> GetOrderBookAsync(string figi, CancellationToken ct = default)
    {
        await _rateLimiter.WaitAsync(ct);
        var orderBook = (await _client.MarketData.GetOrderBookAsync(new GetOrderBookRequest()
        {
            Depth = 10,
            InstrumentId = figi,
        }, cancellationToken: ct));
        
        var bids = orderBook.Bids
            .Select(x => x.Price * x.Quantity)
            .Sum();
        
        var asks = orderBook.Asks
            .Select(x => x.Price * x.Quantity)
            .Sum();
        
        var totalVolume = bids + asks;
        var bidImbalance = totalVolume > 0 ? bids / totalVolume : 0;
        var askImbalance = totalVolume > 0 ? asks / totalVolume : 0;
        
        var bestBid = orderBook.Bids.Max(x => (decimal)x.Price);
        var bestAsk = orderBook.Asks.Min(x => (decimal)x.Price);

        var midPrice = (bestAsk + bestBid) / 2m;
        var spreadBps = midPrice > 0 ? (bestAsk - bestBid) / midPrice * 10_000m : 0;

        return new OrderBookDto
        {
            Figi = orderBook.Figi,
            AskVol = asks,
            BidVol = bids,
            AskImbalance = askImbalance,
            BidImbalance = bidImbalance,
            SpreadBps = spreadBps,
        };
    }
}