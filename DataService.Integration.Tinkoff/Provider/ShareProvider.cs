using AutoMapper;
using DataService.Integration.Interfaces;
using DataService.Integration.Models;
using DataService.Integration.Tinkoff.Limiters;
using Tinkoff.InvestApi;

namespace DataService.Integration.Tinkoff.Provider;

/// <summary>
/// Реализация провайдера получения акций
/// </summary>
public class ShareProvider(InvestApiClient client, MarketDataRateLimiter limiter, IMapper mapper) : IShareProvider
{
    /// <summary>
    /// Получить акции
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<IReadOnlyCollection<ShareDto>> GetSharesAsync(CancellationToken cancellationToken = default)
    {
        await limiter.WaitAsync(cancellationToken);

        var shares = (await client.Instruments.SharesAsync())
            .Instruments
            .ToArray()
            .AsReadOnly();
        return mapper.Map<IReadOnlyCollection<ShareDto>>(shares);
    }
}