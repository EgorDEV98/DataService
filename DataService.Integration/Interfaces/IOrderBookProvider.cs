using DataService.Integration.Models.Request;
using DataService.Integration.Models.Response;

namespace DataService.Integration.Interfaces;

/// <summary>
/// Провайдер получения стакана
/// </summary>
public interface IOrderBookProvider
{
    /// <summary>
    /// Получить стакан
    /// </summary>
    /// <param name="request">Параметры</param>
    /// <param name="cancellationToken">Токен</param>
    /// <returns></returns>
    Task<ExternalGetOrderBookResponse> GetOrderBookAsync(ExternalGetOrderBookRequest request, CancellationToken cancellationToken = default);
}