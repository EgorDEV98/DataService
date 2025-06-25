using DataService.Application.Interfaces;
using DataService.Application.Models;
using DataService.Contracts.Clients;
using DataService.Contracts.Models.Request;
using DataService.Contracts.Models.Response;
using DataService.WebApi.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace DataService.WebApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class OrderBooksController(IOrderBookService service, OrderBookMapper mapper) : ControllerBase, IOrderBooksClient
{
    /// <summary>
    /// Получить стакан по FIGI
    /// </summary>
    /// <param name="request">Параметры</param>
    /// <param name="cancellationToken">Токен</param>
    /// <returns></returns>
    [HttpGet("getByFigi")]
    public async Task<GetOrderBookResponse> GetOrderBookByFigiAsync(GetOrderBookByFigiRequest request, CancellationToken cancellationToken = default)
    {
        var result = await service.GetOrderBookByFigiAsync(new GetOrderBookParams()
        {
            Figi = request.Figi,
            Depth = request.Depth
        }, cancellationToken);
        return mapper.Map(result);
    }

    /// <summary>
    /// Получить стакан по Id акции
    /// </summary>
    /// <param name="request">Параметры</param>
    /// <param name="cancellationToken">Токен</param>
    /// <returns></returns>
    [HttpGet("getById")]
    public async Task<GetOrderBookResponse> GetOrderBookByShareIdAsync(GetOrderBookByIdRequest request, CancellationToken cancellationToken = default)
    {
        var result = await service.GetOrderBookByIdAsync(new GetOrderBookByIdParams()
        {
            Id = request.Id,
            Depth = request.Depth
        }, cancellationToken);
        return mapper.Map(result);
    }
}