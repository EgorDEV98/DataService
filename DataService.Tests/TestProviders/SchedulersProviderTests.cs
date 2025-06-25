using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataService.Integration.Models.Request;
using DataService.Integration.Models.Response;
using DataService.Integration.Tinkoff.Common;
using DataService.Integration.Tinkoff.Providers;
using Google.Protobuf.WellKnownTypes;
using Moq;
using Tinkoff.InvestApi.V1;
using Xunit;
using Grpc.Core;

namespace DataService.Tests.TestProviders;

public class SchedulersProviderTests
{
    private static readonly ExternalGetTradingSchedulers SampleRequest = new()
    {
        Exchange = "MOEX",
        From = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
        To = new DateTimeOffset(2024, 1, 3, 0, 0, 0, TimeSpan.Zero)
    };

    [Fact]
    public async Task GetTradingSchedulersAsync_ReturnsMappedSchedulers()
    {
        // Arrange
        var grpcResponse = new TradingSchedulesResponse
        {
            Exchanges =
            {
                new TradingSchedule
                {
                    Exchange = "MOEX",
                    Days =
                    {
                        new TradingDay
                        {
                            IsTradingDay = true,
                            StartTime = Timestamp.FromDateTimeOffset(SampleRequest.From.Value),
                            EndTime = Timestamp.FromDateTimeOffset(SampleRequest.To.Value)
                        }
                    }
                }
            }
        };

        var grpcClientMock = new Mock<InstrumentsService.InstrumentsServiceClient>();
        grpcClientMock
            .Setup(x => x.TradingSchedulesAsync(It.Is<TradingSchedulesRequest>(r =>
                r.Exchange == "MOEX" &&
                r.From.Equals(Timestamp.FromDateTimeOffset(SampleRequest.From.Value)) &&
                r.To.Equals(Timestamp.FromDateTimeOffset(SampleRequest.To.Value))
            ), null, null, default))
            .Returns(CreateUnaryCall(grpcResponse));

        var limiterMock = new Mock<InstrumentRateLimiter>();
        limiterMock.Setup(x => x.WaitAsync(default)).Returns(Task.CompletedTask);

        var provider = new SchedulersProvider(grpcClientMock.Object, limiterMock.Object);

        // Act
        var result = await provider.GetTradingSchedulersAsync(SampleRequest, default);

        // Assert
        Assert.Single(result);
        var scheduler = result.First();
        Assert.Equal("MOEX", scheduler.Exchange);
        Assert.True(scheduler.IsTradingDay);
        Assert.Equal(SampleRequest.From.Value, scheduler.StartTime);
        Assert.Equal(SampleRequest.To.Value, scheduler.EndTime);
    }

    [Fact]
    public async Task GetTradingSchedulersAsync_ThrowsIfLimiterFails()
    {
        // Arrange
        var grpcClientMock = new Mock<InstrumentsService.InstrumentsServiceClient>();
        var limiterMock = new Mock<InstrumentRateLimiter>();
        limiterMock.Setup(x => x.WaitAsync(default))
            .ThrowsAsync(new TaskCanceledException("Rate limit exceeded"));

        var provider = new SchedulersProvider(grpcClientMock.Object, limiterMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(() =>
            provider.GetTradingSchedulersAsync(SampleRequest, default));
    }

    [Fact]
    public async Task GetTradingSchedulersAsync_ReturnsEmptyIfNoExchanges()
    {
        // Arrange
        var grpcResponse = new TradingSchedulesResponse(); // No exchanges

        var grpcClientMock = new Mock<InstrumentsService.InstrumentsServiceClient>();
        grpcClientMock
            .Setup(x => x.TradingSchedulesAsync(It.IsAny<TradingSchedulesRequest>(), null, null, default))
            .Returns(CreateUnaryCall(grpcResponse));

        var limiterMock = new Mock<InstrumentRateLimiter>();
        limiterMock.Setup(x => x.WaitAsync(default)).Returns(Task.CompletedTask);

        var provider = new SchedulersProvider(grpcClientMock.Object, limiterMock.Object);

        // Act
        var result = await provider.GetTradingSchedulersAsync(SampleRequest, default);

        // Assert
        Assert.Empty(result);
    }

    private static AsyncUnaryCall<T> CreateUnaryCall<T>(T response) where T : class =>
        new AsyncUnaryCall<T>(
            Task.FromResult(response),
            Task.FromResult(new Metadata()),
            () => new Status(StatusCode.OK, ""),
            () => new Metadata(),
            () => { });
}
