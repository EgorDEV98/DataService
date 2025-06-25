using DataService.Integration.Models.Response;
using Riok.Mapperly.Abstractions;
using Tinkoff.InvestApi.V1;

namespace DataService.Integration.Tinkoff.Mappers;

[Mapper]
public partial class ExternalOrderBookMapper
{
    public partial ExternalGetOrderBookResponse Map(GetOrderBookResponse request);
}