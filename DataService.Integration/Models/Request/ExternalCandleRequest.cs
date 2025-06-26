using DataService.Contracts.Models.Enums;

namespace DataService.Integration.Models.Request;

public class ExternalCandleRequest
{
    /// <summary>
    /// Фиги инструмента
    /// </summary>
    public string Figi { get; set; }
    
    /// <summary>
    /// Начало запрашиваемого периода по UTC
    /// </summary>
    public DateTimeOffset From { get; set; }
    
    /// <summary>
    /// Окончание запрашиваемого периода по UTC
    /// </summary>
    public DateTimeOffset To { get; set; }
    
    /// <summary>
    /// Интервал свечи
    /// </summary>
    public Interval Interval { get; set; }
}