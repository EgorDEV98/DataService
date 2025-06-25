using DataService.Application.Models;
using DataService.Data.Entities;

namespace DataService.Application.Interfaces;

/// <summary>
/// Сервис предоставления режима работы биржи
/// </summary>
public interface ISchedulersService
{
    /// <summary>
    /// Получить список режимов работы
    /// </summary>
    /// <param name="param">Параметры</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<IReadOnlyCollection<Scheduler>> GetSchedulersAsync(GetSchedulersParams param, CancellationToken cancellationToken);

    /// <summary>
    /// Выполнить предзагрузку всех режимов в БД
    /// </summary>
    /// <param name="param">Параметры</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<bool> PreloadSchedulersAsync(PreloadSchedulerParams param, CancellationToken cancellationToken);
}