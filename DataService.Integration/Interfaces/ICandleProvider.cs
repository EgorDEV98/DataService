using DataService.Integration.Enums;
using DataService.Integration.Models;

namespace DataService.Integration.Interfaces;

/// <summary>
/// Провайдер получния свечей
/// </summary>
public interface ICandleProvider
{
    /// <summary>
    /// Получить свечи п
    /// </summary>
    /// <param name="figi">Фиги</param>
    /// <param name="interval">Интервал свечей</param>
    /// <param name="from">С какого момента</param>
    /// <param name="to">До какого момента</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<IReadOnlyCollection<CandleDto>> GetCandlesAsync(string figi, CandleInterval interval, DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken);
}