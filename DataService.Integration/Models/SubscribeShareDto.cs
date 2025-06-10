using DataService.Contracts.Models.Enums;

namespace DataService.Integration.Models;

public class SubscribeShareDto
{
    /// <summary>
    /// Фиги инструмента
    /// </summary>
    public string Figi { get; set; }
    
    /// <summary>
    /// Интервал
    /// </summary>
    public Interval Interval { get; set; }
}