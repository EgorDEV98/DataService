namespace DataService.Application.Options;

/// <summary>
/// Настройки сессии
/// </summary>
public class SessionOptions
{
    /// <summary>
    /// Время начала основной сессии (в формате HH:mm, UTC+3)
    /// </summary>
    public required TimeSpan SessionStartTime { get; init; }

    /// <summary>
    /// Время окончания основной сессии (в формате HH:mm, UTC+3)
    /// </summary>
    public required TimeSpan SessionEndTime { get; init; }
}