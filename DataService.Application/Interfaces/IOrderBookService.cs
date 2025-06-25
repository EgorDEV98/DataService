using DataService.Application.Models;
using DataService.Integration.Models.Response;

namespace DataService.Application.Interfaces;

/// <summary>
/// Сервис предоставления данных по стакану
/// </summary>
public interface IOrderBookService
{
    /// <summary>
    /// Получить стакан
    /// </summary>
    /// <param name="param">Параметры</param>
    /// <param name="cancellationToken">Токен</param>
    /// <returns></returns>
    public Task<ExternalGetOrderBookResponse> GetOrderBookByFigiAsync(GetOrderBookParams param, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получить стакан по Id акции
    /// </summary>
    /// <param name="param">Параметры</param>
    /// <param name="cancellationToken">Токен</param>
    /// <returns></returns>
    public Task<ExternalGetOrderBookResponse> GetOrderBookByIdAsync(GetOrderBookByIdParams param, CancellationToken cancellationToken = default);
}