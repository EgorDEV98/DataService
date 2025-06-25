using DataService.Integration.Models.Request;
using DataService.Integration.Models.Response;
using DataService.Integration.Tinkoff.Common;
using DataService.Integration.Tinkoff.Mappers;
using DataService.Integration.Tinkoff.Providers;
using Grpc.Core;
using Moq;
using Tinkoff.InvestApi.V1;

namespace DataService.Tests.TestProviders;

public class OrderBookProviderTests
{
    private static readonly ExternalGetOrderBookRequest SampleRequest = new()
    {
        Figi = "BBG004730ZJ9",
        Depth = 5
    };

    private static readonly GetOrderBookResponse SampleGrpcResponse = new()
    {
        Figi = "BBG004730ZJ9",
        Depth = 5
    };

    [Fact]
    public async Task GetOrderBookAsync_ReturnsMappedOrderBook()
    {
        // Arrange
        var grpcClientMock = new Mock<MarketDataService.MarketDataServiceClient>(MockBehavior.Strict);
        grpcClientMock
            .Setup(x => x.GetOrderBookAsync(It.Is<GetOrderBookRequest>(r =>
                r.InstrumentId == SampleRequest.Figi &&
                r.Depth == SampleRequest.Depth
            ), null, null, default))
            .Returns(CreateUnaryCall(SampleGrpcResponse));

        var limiterMock = new Mock<MarketDataRateLimiter>();
        limiterMock.Setup(x => x.WaitAsync(default)).Returns(Task.CompletedTask);
        
        var provider = new OrderBookProvider(grpcClientMock.Object, limiterMock.Object, new ExternalOrderBookMapper());

        // Act
        var result = await provider.GetOrderBookAsync(SampleRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(SampleRequest.Figi, result.Figi);
    }

    [Fact]
    public async Task GetOrderBookAsync_ThrowsIfLimiterFails()
    {
        // Arrange
        var grpcClientMock = new Mock<MarketDataService.MarketDataServiceClient>();
        var limiterMock = new Mock<MarketDataRateLimiter>();
        limiterMock.Setup(x => x.WaitAsync(default)).ThrowsAsync(new TaskCanceledException("Rate limited"));

        var mapperMock = new Mock<ExternalOrderBookMapper>();

        var provider = new OrderBookProvider(grpcClientMock.Object, limiterMock.Object, mapperMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(() =>
            provider.GetOrderBookAsync(SampleRequest));
    }

    [Fact]
    public async Task GetOrderBookAsync_ThrowsIfGrpcFails()
    {
        // Arrange
        var grpcClientMock = new Mock<MarketDataService.MarketDataServiceClient>();
        grpcClientMock
            .Setup(x => x.GetOrderBookAsync(It.IsAny<GetOrderBookRequest>(), null, null, default))
            .Throws(new RpcException(new Status(StatusCode.Internal, "grpc error")));

        var limiterMock = new Mock<MarketDataRateLimiter>();
        limiterMock.Setup(x => x.WaitAsync(default)).Returns(Task.CompletedTask);

        var mapperMock = new Mock<ExternalOrderBookMapper>();

        var provider = new OrderBookProvider(grpcClientMock.Object, limiterMock.Object, mapperMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<RpcException>(() =>
            provider.GetOrderBookAsync(SampleRequest));
    }

    [Fact]
    public async Task GetOrderBookAsync_ThrowsIfGrpcReturnsNull()
    {
        var grpcClientMock = new Mock<MarketDataService.MarketDataServiceClient>();
        grpcClientMock
            .Setup(x => x.GetOrderBookAsync(It.IsAny<GetOrderBookRequest>(), null, null, default))
            .Returns(CreateUnaryCall<GetOrderBookResponse>(null));

        var limiterMock = new Mock<MarketDataRateLimiter>();
        limiterMock.Setup(x => x.WaitAsync(default)).Returns(Task.CompletedTask);

        var provider = new OrderBookProvider(grpcClientMock.Object, limiterMock.Object, new ExternalOrderBookMapper());

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            provider.GetOrderBookAsync(SampleRequest));

        Assert.Contains("null OrderBook response", ex.Message);
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
