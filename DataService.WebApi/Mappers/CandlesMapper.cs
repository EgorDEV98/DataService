using DataService.Application.Models;
using DataService.Contracts.Models.Request;
using DataService.Contracts.Models.Response;
using DataService.Data.Entities;
using Riok.Mapperly.Abstractions;

namespace DataService.WebApi.Mappers;

[Mapper]
public partial class CandlesMapper
{
    public partial GetCandlesParams Map(GetCandlesRequest request);
    public partial IReadOnlyCollection<GetCandleResponse> Map(IEnumerable<Candle> request);
}