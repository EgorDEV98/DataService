using DataService.Contracts.Models.Request;
using DataService.Contracts.Models.Response;
using Refit;

namespace DataService.Contracts.Clients;

/// <summary>
/// Клиент получения свечей
/// </summary>
public interface ICandlesClient
{
    /// <summary>
    /// Получить список свечей
    /// </summary>
    /// <param name="request">Параметры запроса</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    [Get("/api/candles")]
    Task<IReadOnlyCollection<GetCandleResponse>> GetCandlesAsync([Query] GetCandlesRequest request, CancellationToken cancellationToken = default);
}