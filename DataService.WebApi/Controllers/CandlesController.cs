using DataService.Application.Interfaces;
using DataService.Contracts.Clients;
using DataService.Contracts.Models.Request;
using DataService.Contracts.Models.Response;
using DataService.WebApi.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace DataService.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CandlesController(ICandlesService service, CandlesMapper mapper) : ControllerBase, ICandlesClient
{
    /// <summary>
    /// Получить список свечей
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpGet]
    public async Task<IReadOnlyCollection<GetCandleResponse>> GetCandlesAsync([FromQuery] GetCandlesRequest request, CancellationToken cancellationToken = default)
    {
        var param = mapper.Map(request);
        var result = await service.GetCandlesAsync(param, cancellationToken);
        return mapper.Map(result);
    }
}