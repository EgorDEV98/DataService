using DataService.Integration.Models.Response;
using Riok.Mapperly.Abstractions;
using Tinkoff.InvestApi.V1;

namespace DataService.Integration.Tinkoff.Mappers;

[Mapper]
public partial class ExternalShareMapper
{
    public partial ExternalGetShareResponse Map(Share share);
    public partial IReadOnlyCollection<ExternalGetShareResponse> Map(IEnumerable<Share> share);
}