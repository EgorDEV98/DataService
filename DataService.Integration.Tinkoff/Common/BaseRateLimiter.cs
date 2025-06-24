using System.Threading.RateLimiting;

namespace DataService.Integration.Tinkoff.Common;

public class BaseRateLimiter<T>(int tokenLimit, int tokensPerPeriod, int queueLimit)
    where T : class
{
    private readonly TokenBucketRateLimiter _limiter = new(new TokenBucketRateLimiterOptions
    {
        TokenLimit = tokenLimit,
        TokensPerPeriod = tokensPerPeriod,
        ReplenishmentPeriod = TimeSpan.FromMinutes(1),
        AutoReplenishment = true,
        QueueLimit = queueLimit,
        QueueProcessingOrder = QueueProcessingOrder.OldestFirst
    });

    /// <summary>
    /// Ожидание
    /// </summary>
    /// <param name="ct"></param>
    /// <exception cref="Exception"></exception>
    public virtual async Task WaitAsync(CancellationToken ct)
    {
        var lease = await _limiter.AcquireAsync(1, ct);
        if (!lease.IsAcquired)
        {
            throw new Exception(nameof(T)+ " Rate limit exceeded");
        }
    }
}