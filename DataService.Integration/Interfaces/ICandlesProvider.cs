using DataService.Integration.Models.Request;
using DataService.Integration.Models.Response;

namespace DataService.Integration.Interfaces;

/// <summary>
/// Провайдер получения свечей
/// </summary>
public interface ICandlesProvider
{
    /// <summary>
    /// Получить список исторических свечей
    /// </summary>
    /// <param name="request">Параметры</param>
    /// <param name="cancellationToken">Токен</param>
    /// <returns></returns>
    Task<IReadOnlyCollection<ExternalCandleResponse>> GetHistoryCandlesAsync(
        ExternalCandleRequest request, 
        CancellationToken cancellationToken = default);
}