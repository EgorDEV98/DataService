using DataService.Application.Models;
using DataService.Application.Services;
using DataService.Contracts.Clients;
using DataService.Contracts.Models.Request;
using DataService.Contracts.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace DataService.WebApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class CandlesController(CandlesService service) : ControllerBase, ICandlesClient
{
    /// <summary>
    /// Получить список свечей
    /// </summary>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpGet]
    public async Task<IReadOnlyCollection<CandleResponse>> GetCandlesAsync([FromQuery] GetCandlesRequest request,
        CancellationToken ct = default)
        => await service.GetCandlesAsync(new GetCandlesParams
        {
            ShareId = request.ShareId,
            OnlyMainSession = request.OnlyMainSession,
            ExcludeWeekend = request.ExcludeWeekend,
            Offset = request.Offset,
            Limit = request.Limit,
            From = request.From,
            To = request.To,
            Interval = request.Interval,
        }, ct);
}