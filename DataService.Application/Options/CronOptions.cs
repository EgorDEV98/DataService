namespace DataService.Application.Options;

/// <summary>
/// Настройки крона
/// </summary>
public class CronOptions
{
    /// <summary>
    /// Крон запуска для ночной синхронизации свечей
    /// </summary>
    public string SyncHistoryCandlesCron { get; set; } = null!;
    
    /// <summary>
    /// Время начала основной сессии (в формате HH:mm, UTC+3)
    /// </summary>
    public required TimeSpan SessionStartTime { get; init; }

    /// <summary>
    /// Время окончания основной сессии (в формате HH:mm, UTC+3)
    /// </summary>
    public required TimeSpan SessionEndTime { get; init; }
}