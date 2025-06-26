using DataService.Application.Models;
using DataService.Data.Entities;

namespace DataService.Application.Interfaces;

/// <summary>
/// Сервис получения свечей
/// </summary>
public interface ICandlesService
{
    /// <summary>
    /// Получить список свечей
    /// </summary>
    /// <param name="param">Параметры</param>
    /// <param name="cancellationToken">Токен</param>
    /// <returns></returns>
    public Task<IReadOnlyCollection<Candle>> GetCandlesAsync(GetCandlesParams param, CancellationToken cancellationToken = default);
}