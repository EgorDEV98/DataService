namespace DataService.Application.Interfaces;

/// <summary>
/// Интерфейс сервиса синхрониации акций
/// </summary>
public interface ISyncShareService
{
    /// <summary>
    /// Синхронизировать акции
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    public Task SyncSharesAsync(CancellationToken ct = default);
}