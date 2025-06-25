using DataService.Contracts.Models.Request;
using DataService.Contracts.Models.Response;
using Refit;

namespace DataService.Contracts.Clients;

public interface ISharesClient
{
    [Get("/api/Shares/{id}")]
    public Task<GetShareResponse> GetShareAsync(Guid id, CancellationToken cancellationToken = default);
    
    [Get("/api/Shares")]
    public Task<IReadOnlyCollection<GetShareResponse>> GetSharesAsync([Query] GetSharesRequest request, CancellationToken cancellationToken = default);
    
    [Patch("/api/Shares/{id}")]
    public Task<bool> ChangeLoadStatusAsync(Guid id, [Body] ChangeLoadStatusRequest request, CancellationToken cancellationToken = default);
}