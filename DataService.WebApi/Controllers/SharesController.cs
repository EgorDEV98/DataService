using DataService.Application.Interfaces;
using DataService.Application.Models;
using DataService.Contracts.Clients;
using DataService.Contracts.Models.Enums;
using DataService.Contracts.Models.Request;
using DataService.Contracts.Models.Response;
using DataService.WebApi.Mappers;
using Microsoft.AspNetCore.Mvc;
using Refit;

namespace DataService.WebApi.Controllers;

[ApiController]
[Route("/api/V1/[controller]")]
public class SharesController(IShareService service, ShareMapper mapper) : ControllerBase, ISharesClient
{
    /// <summary>
    /// Получить акцию по Id
    /// </summary>
    /// <param name="id">Идентификатор акции </param>
    /// <param name="cancellationToken">Токен</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<GetShareResponse> GetShareAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await service.GetShareAsync(new GetShareParams()
        { Id = id }, cancellationToken);
        return mapper.Map(result);
    }

    /// <summary>
    /// Получить список акций
    /// </summary>
    /// <param name="request">Параметры</param>
    /// <param name="cancellationToken">Токен</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IReadOnlyCollection<GetShareResponse>> GetSharesAsync([FromQuery] GetSharesRequest request, CancellationToken cancellationToken = default)
    {
        var param = mapper.Map(request);
        var result = await service.GetSharesAsync(param, cancellationToken);
        return mapper.Map(result);
    }

    /// <summary>
    /// Изменить статус загрузки
    /// </summary>
    /// <param name="id">Идентификатор акции</param>
    /// <param name="request">Параметры</param>
    /// <param name="cancellationToken">Токен</param>
    /// <returns></returns>
    [HttpPatch("{id}")]
    public async Task<bool> ChangeLoadStatusAsync([FromRoute] Guid id, [Body] ChangeLoadStatusRequest request, CancellationToken cancellationToken = default)
        => await service.ChangeLoadStatus(new ChangeLoadStatusParams
        {
            Id = id,
            CandleLoadStatus = request.CandleLoadStatus
        }, cancellationToken);
}