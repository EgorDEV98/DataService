using DataService.Integration.Models;

namespace DataService.Integration.Interfaces;

/// <summary>
/// Провайдер стакана
/// </summary>
public interface IOrderBookProvider
{
    /// <summary>
    /// Получить стакан
    /// </summary>
    /// <param name="figi">Фиги</param>
    /// <param name="orderBookDepth">Глубина стакана</param>
    /// <param name="ct">Токен</param>
    /// <returns></returns>
    Task<OrderBookDto> GetOrderBookAsync(string figi, int orderBookDepth, CancellationToken ct = default);
}