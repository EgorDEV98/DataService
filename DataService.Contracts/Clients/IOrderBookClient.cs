using DataService.Contracts.Models.Response;
using Refit;

namespace DataService.Contracts.Clients;

public interface IOrderBookClient
{
    [Get("/OrderBook/{shareId}")]
    Task<OrderBookResponse> GetOrderBook(Guid shareId, [Query] int depth, CancellationToken cancellationToken = default);
}