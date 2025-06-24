using AutoFixture;
using DataService.Integration.Tinkoff.Mappers;
using Moq;
using Tinkoff.InvestApi.V1;

namespace DataService.Tests.TestMappers;

public class TinkoffShareMapperTests
{
    private readonly Fixture _fixture;
    private readonly ExternalShareMapper _mapper;

    public TinkoffShareMapperTests()
    {
        _fixture = new Fixture();
        _mapper = new ExternalShareMapper();
    }
    
    [Fact]
    public void TinkoffShareMapperTest()
    {
        var responseValue = _fixture.Create<Share>();
        var mappedValue = _mapper.Map(responseValue);
        
        Assert.Equal(mappedValue.Ticker, responseValue.Ticker);
        Assert.Equal(mappedValue.ClassCode, responseValue.ClassCode);
        Assert.Equal(mappedValue.Lot, responseValue.Lot);
        Assert.Equal(mappedValue.Currency, responseValue.Currency);
        Assert.Equal(mappedValue.ShortEnabledFlag, responseValue.ShortEnabledFlag);
        Assert.Equal(mappedValue.Name, responseValue.Name);
        Assert.Equal(mappedValue.CountryOfRisk, responseValue.CountryOfRisk);
        Assert.Equal(mappedValue.CountryOfRiskName, responseValue.CountryOfRiskName);
        Assert.Equal(mappedValue.Sector, responseValue.Sector);
        Assert.Equal(mappedValue.DivYieldFlag, responseValue.DivYieldFlag);
        Assert.Equal(mappedValue.MinPriceIncrement, (decimal)responseValue.MinPriceIncrement);
        Assert.Equal(mappedValue.WeekendFlag, responseValue.WeekendFlag);
        Assert.Equal(mappedValue.First1MinCandleDate, responseValue.First1MinCandleDate.ToDateTimeOffset());
        Assert.Equal(mappedValue.First1DayCandleDate, responseValue.First1DayCandleDate.ToDateTimeOffset());
    }
    
    [Fact]
    public void TinkoffShareMapperRangeTest()
    {
        var count = 10;
        var responseValue = _fixture.CreateMany<Share>(count).ToList();
        var mappedValue = _mapper.Map(responseValue);
        
        Assert.Equal(count, mappedValue.Count);
        foreach (var pair in mappedValue.Zip(responseValue))
        {
            
            Assert.Equal(pair.First.Ticker, pair.Second.Ticker);
            Assert.Equal(pair.First.ClassCode, pair.Second.ClassCode);
            Assert.Equal(pair.First.Lot, pair.Second.Lot);
            Assert.Equal(pair.First.Currency, pair.Second.Currency);
            Assert.Equal(pair.First.ShortEnabledFlag, pair.Second.ShortEnabledFlag);
            Assert.Equal(pair.First.Name, pair.Second.Name);
            Assert.Equal(pair.First.CountryOfRisk, pair.Second.CountryOfRisk);
            Assert.Equal(pair.First.CountryOfRiskName, pair.Second.CountryOfRiskName);
            Assert.Equal(pair.First.Sector, pair.Second.Sector);
            Assert.Equal(pair.First.DivYieldFlag, pair.Second.DivYieldFlag);
            Assert.Equal(pair.First.MinPriceIncrement, (decimal)pair.Second.MinPriceIncrement);
            Assert.Equal(pair.First.WeekendFlag, pair.Second.WeekendFlag);
            Assert.Equal(pair.First.First1MinCandleDate, pair.Second.First1MinCandleDate.ToDateTimeOffset());
            Assert.Equal(pair.First.First1DayCandleDate, pair.Second.First1DayCandleDate.ToDateTimeOffset());
        }
    }
}