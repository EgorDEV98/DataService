using DataService.Contracts.Models.Enums;
using DataService.Integration.Models;

namespace DataService.Integration.Interfaces;

/// <summary>
/// Провайдер получения свечей
/// </summary>
public interface ICandleProvider
{
    /// <summary>
    /// Получить список свечей
    /// </summary>
    /// <param name="figi">Фиги</param>
    /// <param name="interval">Интервал свечей</param>
    /// <param name="from">С какого момента</param>
    /// <param name="to">До какого момента</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<IReadOnlyCollection<CandleDto>> GetCandlesAsync(string figi, Interval interval, DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken);
}