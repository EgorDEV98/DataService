using DataService.Integration.Models;

namespace DataService.Integration.Interfaces;

/// <summary>
/// Провайдер акций
/// </summary>
public interface IShareProvider
{
    /// <summary>
    /// Получить список акций
    /// </summary>
    /// <param name="cancellationToken">Токен</param>
    /// <returns></returns>
    public Task<IReadOnlyCollection<ShareDto>> GetSharesAsync(CancellationToken cancellationToken = default);
}