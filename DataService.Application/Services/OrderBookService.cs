using AutoMapper;
using DataService.Application.Models;
using DataService.Contracts.Models.Response;
using DataService.Data;
using DataService.Integration.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataService.Application.Services;

public class OrderBookService(PostgresDbContext context, IOrderBookProvider orderBookProvider, IMapper mapper)
{
    public async Task<OrderBookResponse> GetOrderBook(GetOrderBookParams param, CancellationToken cancellationToken)
    {
        var share = await context.Shares
            .FirstOrDefaultAsync(x => x.Id == param.ShareId, cancellationToken);
        if(share == null) throw new KeyNotFoundException("Share not found");
        
        var orderBook = await orderBookProvider.GetOrderBookAsync(share.Figi, param.Depth, cancellationToken);
        return mapper.Map<OrderBookResponse>(orderBook);
    }
}