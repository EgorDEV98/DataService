using AutoFixture;
using DataService.Integration.Tinkoff.Mappers;
using Tinkoff.InvestApi.V1;

namespace DataService.Tests.TestMappers;

public class TinkoffOrderBookMapperTests
{
    private readonly Fixture _fixture;
    private readonly ExternalOrderBookMapper _mapper;

    public TinkoffOrderBookMapperTests()
    {
        _fixture = new Fixture();
        _mapper = new ExternalOrderBookMapper();
    }

    [Fact]
    public void Map_OrderBook()
    {
        var asks = _fixture.CreateMany<Order>(5);
        var bids = _fixture.CreateMany<Order>(5);
        var externalResponse = _fixture.Build<GetOrderBookResponse>()
            .Do(x => x.Asks.Add(asks))
            .Do(x => x.Bids.Add(bids))
            .Create();
        
        var mapped = _mapper.Map(externalResponse);
        
        Assert.NotNull(mapped);
        Assert.NotNull(externalResponse);
        Assert.Equal(mapped.Depth, externalResponse.Depth);
        Assert.Equal(mapped.Figi, externalResponse.Figi);
        
        Assert.Equal(mapped.Asks.Count, externalResponse.Asks.Count);
        Assert.Equal(mapped.Bids.Count, externalResponse.Bids.Count);

        foreach (var ask in mapped.Asks.Zip(externalResponse.Asks))
        {
            Assert.Equal(ask.First.Price, (decimal)ask.Second.Price);
            Assert.Equal(ask.First.Quantity, ask.Second.Quantity);
        }
    }
}