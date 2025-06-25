namespace DataService.Contracts.Models.Request;

public class GetSchedulersRequest
{
    /// <summary>
    /// Биржа
    /// </summary>
    public string? Exchange { get; set; }
    
    /// <summary>
    /// Признак рабочего дня
    /// </summary>
    public bool? IsTradingDay { get; set; }
    
    /// <summary>
    /// Начала работы в UTC
    /// </summary>
    public DateTimeOffset? StartTime { get; set; }
    
    /// <summary>
    /// Окончание работы в UTC
    /// </summary>
    public DateTimeOffset? EndTime { get; set; }
}