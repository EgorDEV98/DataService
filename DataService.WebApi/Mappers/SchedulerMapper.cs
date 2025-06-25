using DataService.Contracts.Models.Response;
using DataService.Data.Entities;
using Riok.Mapperly.Abstractions;

namespace DataService.WebApi.Mappers;

[Mapper]
public partial class SchedulerMapper
{
    public partial GetSchedulersResponse Map(Scheduler scheduler);
    public partial IReadOnlyCollection<GetSchedulersResponse> Map(IEnumerable<Scheduler> schedulers);
}