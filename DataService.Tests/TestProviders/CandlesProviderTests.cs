using AutoFixture;
using DataService.Contracts.Models.Enums;
using DataService.Integration.Models.Request;
using DataService.Integration.Tinkoff.Common;
using DataService.Integration.Tinkoff.Mappers;
using DataService.Integration.Tinkoff.Providers;
using Google.Protobuf.WellKnownTypes;
using Moq;
using Tinkoff.InvestApi.V1;
using Grpc.Core;

namespace DataService.Tests.TestProviders;

public class CandlesProviderTests
{
    private readonly Fixture _fixture;
    private static readonly ExternalCandleRequest SampleRequest = new()
    {
        Figi = "BBG000B9XRY4",
        Interval = Interval._1Min,
        From = new DateTimeOffset(2024, 1, 1, 10, 0, 0, TimeSpan.Zero),
        To = new DateTimeOffset(2024, 1, 1, 10, 10, 0, TimeSpan.Zero)
    };

    public CandlesProviderTests()
    {
        _fixture = new Fixture();
    }
    
    

    [Fact]
    public async Task GetHistoryCandlesAsync_ReturnsMappedCandles()
    {
        var grpcCandles = _fixture.CreateMany<HistoricCandle>(1);

        var grpcResponse = new GetCandlesResponse();
        grpcResponse.Candles.AddRange(grpcCandles);

        var grpcClientMock = new Mock<MarketDataService.MarketDataServiceClient>();
        grpcClientMock
            .Setup(x => x.GetCandlesAsync(It.Is<GetCandlesRequest>(r =>
                r.InstrumentId == SampleRequest.Figi &&
                r.Interval == CandleInterval._1Min &&
                r.From == Timestamp.FromDateTimeOffset(SampleRequest.From) &&
                r.To == Timestamp.FromDateTimeOffset(SampleRequest.To)
            ), null, null, default))
            .Returns(CreateUnaryCall(grpcResponse));

        var limiterMock = new Mock<MarketDataRateLimiter>();
        limiterMock.Setup(x => x.WaitAsync(CancellationToken.None)).Returns(Task.CompletedTask);
        
        var provider = new CandlesProvider(grpcClientMock.Object, limiterMock.Object, new ExternalCandlesMapper());

        // Act
        var result = await provider.GetHistoryCandlesAsync(SampleRequest);

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task GetHistoryCandlesAsync_Throws_WhenLimiterFails()
    {
        // Arrange
        var grpcClientMock = new Mock<MarketDataService.MarketDataServiceClient>();
        var limiterMock = new Mock<MarketDataRateLimiter>();
        limiterMock.Setup(x => x.WaitAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException("Rate limit exceeded"));
        
        var provider = new CandlesProvider(grpcClientMock.Object, limiterMock.Object, new ExternalCandlesMapper());

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            provider.GetHistoryCandlesAsync(SampleRequest));
    }

    private static AsyncUnaryCall<T> CreateUnaryCall<T>(T response) where T : class
    {
        return new AsyncUnaryCall<T>(
            Task.FromResult(response),
            Task.FromResult(new Metadata()),
            () => new Status(StatusCode.OK, string.Empty),
            () => new Metadata(),
            () => { });
    }
}
