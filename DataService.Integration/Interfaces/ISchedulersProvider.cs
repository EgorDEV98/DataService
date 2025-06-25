using DataService.Integration.Models.Request;
using DataService.Integration.Models.Response;

namespace DataService.Integration.Interfaces;

/// <summary>
/// Провайдер получения режима работы биржи
/// </summary>
public interface ISchedulersProvider
{
    /// <summary>
    /// Получить расписание торговых площадок
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<ExternalGetTradingSchedulersResponse>> GetTradingSchedulersAsync(ExternalGetTradingSchedulers request, CancellationToken cancellationToken);
}