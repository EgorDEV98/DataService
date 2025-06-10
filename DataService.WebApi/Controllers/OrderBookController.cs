using DataService.Application.Models;
using DataService.Application.Services;
using DataService.Contracts.Clients;
using DataService.Contracts.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace DataService.WebApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class OrderBookController(OrderBookService service) : ControllerBase, IOrderBookClient
{
    [HttpGet("{shareId}")]
    public async Task<OrderBookResponse> GetOrderBook([FromRoute] Guid shareId, int depth, CancellationToken cancellationToken = default)
        => await service.GetOrderBook(new GetOrderBookParams()
        {
            ShareId = shareId,
            Depth = depth,
        }, cancellationToken);
}