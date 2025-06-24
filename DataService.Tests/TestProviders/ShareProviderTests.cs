using DataService.Integration.Models.Request;
using DataService.Integration.Tinkoff.Common;
using DataService.Integration.Tinkoff.Mappers;
using DataService.Integration.Tinkoff.Providers;
using Grpc.Core;
using Moq;
using Tinkoff.InvestApi.V1;

namespace DataService.Tests.TestProviders;

public class SharesProviderTests
{
    [Fact]
    public async Task GetSharesAsync_ReturnsMappedShares()
    {
        // Arrange
        var response = new SharesResponse
        {
            Instruments = { new Share { Ticker = "SBER", Figi = "BBG004730N88", ClassCode = "TQBR" } }
        };

        var clientMock = new Mock<InstrumentsService.InstrumentsServiceClient>(MockBehavior.Strict);
        clientMock
            .Setup(x => x.SharesAsync(It.IsAny<InstrumentsRequest>(), null, null, CancellationToken.None))
            .Returns(CreateUnaryCall(response));

        var limiter = new Mock<InstrumentRateLimiter>();
        limiter.Setup(x => x.WaitAsync(CancellationToken.None)).Returns(Task.CompletedTask);

        var provider = new SharesProvider(clientMock.Object, limiter.Object, new ExternalShareMapper());

        // Act
        var result = await provider.GetSharesAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("SBER", result.First().Ticker);
    }

    [Fact]
    public async Task GetShareByTickerAsync_ReturnsCorrectShare()
    {
        // Arrange
        var share = new Share { Ticker = "GAZP", Figi = "BBG004730RP0", ClassCode = "TQBR" };
        var response = new ShareResponse { Instrument = share };

        var clientMock = new Mock<InstrumentsService.InstrumentsServiceClient>();
        clientMock
            .Setup(x => x.ShareByAsync(It.IsAny<InstrumentRequest>(), null, null, CancellationToken.None))
            .Returns(CreateUnaryCall(response));

        var limiter = new Mock<InstrumentRateLimiter>();
        limiter.Setup(x => x.WaitAsync(CancellationToken.None)).Returns(Task.CompletedTask);

        var provider = new SharesProvider(clientMock.Object, limiter.Object, new ExternalShareMapper());

        var request = new ExternalGetShareRequest
        {
            Ticker = "GAZP",
            ClassCode = "TQBR"
        };

        // Act
        var result = await provider.GetShareByTickerAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("GAZP", result.Ticker);
    }
    
    private static AsyncUnaryCall<T> CreateUnaryCall<T>(T response) where T : class
    {
        return new AsyncUnaryCall<T>(
            Task.FromResult(response),
            Task.FromResult(new Metadata()),
            () => new Status(StatusCode.OK, string.Empty),
            () => new Metadata(),
            () => { }
        );
    }
}