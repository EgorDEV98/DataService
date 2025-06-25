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
public class SchedulersController(ISchedulersService service, SchedulerMapper mapper) : ControllerBase, ISchedulersClient
{
    [HttpGet]
    public async Task<IReadOnlyCollection<GetSchedulersResponse>> GetSchedulers([FromQuery] GetSchedulersRequest request, CancellationToken cancellationToken = default)
    {
        var result = await service.GetSchedulersAsync(new GetSchedulersParams()
        {
            Exchange = request.Exchange,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            IsTradingDay = request.IsTradingDay,
        }, cancellationToken);
        return mapper.Map(result);
    }
}