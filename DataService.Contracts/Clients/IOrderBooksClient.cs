using DataService.Contracts.Models.Request;
using DataService.Contracts.Models.Response;
using Refit;

namespace DataService.Contracts.Clients;

/// <summary>
/// Клиент стакана
/// </summary>
public interface IOrderBooksClient
{
    /// <summary>
    /// Получить стакан по FIGI
    /// </summary>
    /// <param name="request">Параметры запроса</param>
    /// <param name="cancellationToken">Токен</param>
    /// <returns></returns>
    [Get("/api/OrderBooks/getByFigi")]
    public Task<GetOrderBookResponse> GetOrderBookByFigiAsync([Query] GetOrderBookByFigiRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получить стакан по Идентификатору акции
    /// </summary>
    /// <param name="request">Параметры запроса</param>
    /// <param name="cancellationToken">Токен</param>
    /// <returns></returns>
    [Get("/api/OrderBooks/getById")]
    public Task<GetOrderBookResponse> GetOrderBookByShareIdAsync([Query] GetOrderBookByIdRequest request, CancellationToken cancellationToken = default);
}