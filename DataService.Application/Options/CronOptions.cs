namespace DataService.Application.Options;

/// <summary>
/// Настройки крона
/// </summary>
public class CronOptions
{
    /// <summary>
    /// Крон запуска для ночной синхронизации свечей
    /// </summary>
    public string SyncHistoryCandlesCron { get; set; }
}