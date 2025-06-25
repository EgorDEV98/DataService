namespace DataService.Application.Models;

public class PreloadSchedulerParams
{
    /// <summary>
    /// Биржа
    /// </summary>
    public string? Exchange { get; set; }
    
    /// <summary>
    /// Начало периода UTC
    /// </summary>
    public DateTimeOffset? From { get; set; }
    
    /// <summary>
    /// Окончание периода UTC
    /// </summary>
    public DateTimeOffset? To { get; set; }
}