using DataService.Contracts.Models.Request;
using DataService.Contracts.Models.Response;
using Refit;

namespace DataService.Contracts.Clients;

public interface ISchedulersClient
{
    /// <summary>
    /// Получить список режим работы
    /// </summary>
    /// <param name="request">Параметры запроса</param>
    /// <param name="cancellationToken">Токен</param>
    /// <returns></returns>
    [Get("/api/Schedulers")]
    Task<IReadOnlyCollection<GetSchedulersResponse>> GetSchedulers([Query] GetSchedulersRequest request, CancellationToken cancellationToken = default);
}