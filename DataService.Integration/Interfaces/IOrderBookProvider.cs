using DataService.Integration.Models;

namespace DataService.Integration.Interfaces;

/// <summary>
/// Провайдер стакана
/// </summary>
public interface IOrderBookProvider
{
    Task<OrderBookDto> GetOrderBookAsync(string figi, CancellationToken ct = default);
}