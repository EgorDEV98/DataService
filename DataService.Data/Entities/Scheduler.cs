namespace DataService.Data.Entities;

/// <summary>
/// Расписание торгов
/// </summary>
public class Scheduler
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Наименование биржи
    /// </summary>
    public string Exchange { get; set; }
    
    /// <summary>
    /// Начало торгов
    /// </summary>
    public DateTimeOffset? StartTime { get; set; } 
    
    /// <summary>
    /// Окончание торгов
    /// </summary>
    public DateTimeOffset? EndTime { get; set; }
    
    /// <summary>
    /// Признак, что день является рабочим
    /// </summary>
    public bool IsTradingDay { get; set; }
}