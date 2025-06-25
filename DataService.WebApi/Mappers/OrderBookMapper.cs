using DataService.Contracts.Models.Response;
using DataService.Integration.Models.Response;
using Riok.Mapperly.Abstractions;

namespace DataService.WebApi.Mappers;

[Mapper]
public partial class OrderBookMapper
{
    public partial GetOrderBookResponse Map(ExternalGetOrderBookResponse request);
}