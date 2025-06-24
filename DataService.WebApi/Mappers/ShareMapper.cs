using DataService.Application.Models;
using DataService.Contracts.Models.Request;
using DataService.Contracts.Models.Response;
using DataService.Data.Entities;
using Riok.Mapperly.Abstractions;

namespace DataService.WebApi.Mappers;

[Mapper]
public partial class ShareMapper
{
    public partial GetShareResponse Map(Share share);
    public partial IReadOnlyCollection<GetShareResponse> Map(IEnumerable<Share> share);
    public partial GetSharesParams Map(GetSharesRequest request);
}