using DataService.Application.Models;
using DataService.Data.Entities;

namespace DataService.Application.Interfaces;

/// <summary>
/// Сервис предоставления акций
/// </summary>
public interface IShareService
{
    /// <summary>
    /// Получить акцию по Id
    /// </summary>
    /// <param name="param">Параметры</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Share> GetShareAsync(GetShareParams param, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получить список акций
    /// </summary>
    /// <param name="param">Параметры</param>
    /// <param name="cancellationToken">Токен</param>
    /// <returns></returns>
    public Task<IReadOnlyCollection<Share>> GetSharesAsync(GetSharesParams param, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Изменить статус загрузки
    /// </summary>
    /// <param name="param"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<bool> ChangeLoadStatus(ChangeLoadStatusParams param, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Предварительная выгрузка акций
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<bool> PreloadSharesAsync(CancellationToken cancellationToken = default);
}