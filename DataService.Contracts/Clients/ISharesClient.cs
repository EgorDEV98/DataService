using DataService.Contracts.Models.Request;
using DataService.Contracts.Models.Response;
using Refit;

namespace DataService.Contracts.Clients;

/// <summary>
/// Клиент получения списка акций
/// </summary>
public interface ISharesClient
{
    /// <summary>
    /// Получить список акций
    /// </summary>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [Get("/api/shares")]
    Task<IReadOnlyCollection<ShareResponse>> GetSharesAsync([Query] GetSharesRequest request, CancellationToken ct);
}