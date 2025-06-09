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
    /// <param name="ct">Токен</param>
    /// <returns></returns>
    [Get("/api/candles")]
    public Task<IReadOnlyCollection<CandleResponse>> GetCandlesAsync([Query] GetCandlesRequest request, CancellationToken ct = default);
}