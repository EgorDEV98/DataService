using DataService.Application.Interfaces;
using DataService.Application.Models;
using DataService.Data;
using DataService.Integration.Interfaces;
using DataService.Integration.Models.Request;
using DataService.Integration.Models.Response;
using Microsoft.EntityFrameworkCore;

namespace DataService.Application.Services;

public class OrderBookService(PostgresDbContext context, IOrderBookProvider orderBookProvider) : IOrderBookService
{
    public async Task<ExternalGetOrderBookResponse> GetOrderBookByFigiAsync(GetOrderBookParams param, CancellationToken cancellationToken = default)
        => await orderBookProvider.GetOrderBookAsync(new ExternalGetOrderBookRequest()
        {
            Depth = param.Depth,
            Figi = param.Figi,
        }, cancellationToken);

    public async Task<ExternalGetOrderBookResponse> GetOrderBookByIdAsync(GetOrderBookByIdParams param, CancellationToken cancellationToken = default)
    {
        var share = await context.Shares
            .FirstOrDefaultAsync(x => x.Id == param.Id, cancellationToken);
        if(share == null) throw new KeyNotFoundException($"No order book found with id: {param.Id}");
        
        var figi = share.Figi;
        return await orderBookProvider.GetOrderBookAsync(new ExternalGetOrderBookRequest()
        {
            Figi = figi,
            Depth = param.Depth,
        }, cancellationToken);
    }
}