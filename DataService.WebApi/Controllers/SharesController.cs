using DataService.Application.Models;
using DataService.Application.Services;
using DataService.Contracts.Clients;
using DataService.Contracts.Models.Request;
using DataService.Contracts.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace DataService.WebApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class SharesController(SharesService service) : ControllerBase, ISharesClient
{
    [HttpGet]
    public async Task<IReadOnlyCollection<ShareResponse>> GetSharesAsync([FromQuery] GetSharesRequest request,
        CancellationToken ct)
        => await service.GetSharesAsync(new GetSharesParams()
        {
            IsEnabled = request.IsEnabled,
        }, ct );
}