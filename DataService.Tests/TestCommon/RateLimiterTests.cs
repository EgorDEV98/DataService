using DataService.Integration.Tinkoff.Common;

namespace DataService.Tests.TestCommon;

public class RateLimiterTests
{
    [Fact]
    public async Task WaitAsync_Allows_When_TokensAvailable()
    {
        // Arrange
        var limiter = new TestRateLimitedService(tokenLimit: 1, tokensPerPeriod: 1, queueLimit: 0);

        // Act
        var ex = await Record.ExceptionAsync(() => limiter.WaitAsync(CancellationToken.None));

        // Assert
        Assert.Null(ex);
    }

    [Fact]
    public async Task WaitAsync_Throws_When_LimitExceeded()
    {
        // Arrange
        var limiter = new TestRateLimitedService(tokenLimit: 1, tokensPerPeriod: 1, queueLimit: 0);

        // consume first token
        await limiter.WaitAsync(CancellationToken.None);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() =>
            limiter.WaitAsync(CancellationToken.None));

        Assert.Contains(nameof(TestRateLimitedService), ex.Message);
        Assert.Contains("Rate limit exceeded", ex.Message);
    }

    [Fact]
    public async Task WaitAsync_Throws_When_Cancelled()
    {
        // Arrange
        var limiter = new TestRateLimitedService(tokenLimit: 1, tokensPerPeriod: 1, queueLimit: 1);

        // consume the only token
        await limiter.WaitAsync(CancellationToken.None);

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync(); // отменяем сразу

        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            limiter.WaitAsync(cts.Token));
    }
}
public class TestRateLimitedService : BaseRateLimiter<TestRateLimitedService>
{
    public TestRateLimitedService(int tokenLimit, int tokensPerPeriod, int queueLimit)
        : base(tokenLimit, tokensPerPeriod, queueLimit)
    {
    }
}
