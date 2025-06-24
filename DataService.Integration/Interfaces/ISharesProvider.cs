using DataService.Integration.Models.Request;
using DataService.Integration.Models.Response;

namespace DataService.Integration.Interfaces;

/// <summary>
/// Провайдер акций
/// </summary>
public interface ISharesProvider
{
    /// <summary>
    /// Получить список акций
    /// </summary>
    /// <param name="cancellationToken">Токен</param>
    /// <returns></returns>
    public Task<IReadOnlyCollection<ExternalGetShareResponse>> GetSharesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить акцию по тикеру
    /// </summary>
    /// <param name="request">Запрос</param>
    /// <param name="cancellationToken">Токен</param>
    /// <returns></returns>
    public Task<ExternalGetShareResponse> GetShareByTickerAsync(ExternalGetShareRequest request, CancellationToken cancellationToken = default);
}