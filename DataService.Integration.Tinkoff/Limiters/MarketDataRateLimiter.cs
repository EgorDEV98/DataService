using System.Threading.RateLimiting;

namespace DataService.Integration.Tinkoff.Limiters;

/// <summary>
/// Лимитер для MarketData
/// </summary>
public class MarketDataRateLimiter
{
    private readonly TokenBucketRateLimiter _limiter = new(new TokenBucketRateLimiterOptions
    {
        TokenLimit = 600,
        TokensPerPeriod = 600,
        ReplenishmentPeriod = TimeSpan.FromMinutes(1),
        AutoReplenishment = true,
        QueueLimit = 1000,
        QueueProcessingOrder = QueueProcessingOrder.OldestFirst
    });

    /// <summary>
    /// Ожидание
    /// </summary>
    /// <param name="ct"></param>
    /// <exception cref="Exception"></exception>
    public async Task WaitAsync(CancellationToken ct)
    {
        var lease = await _limiter.AcquireAsync(1, ct);
        if (!lease.IsAcquired)
        {
            throw new Exception("Rate limit exceeded");
        }
    }
}