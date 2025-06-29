using DataService.Contracts.Models.Enums;

namespace DataService.Integration.Models.Request;

public class ExternalSubscribeShareRequest
{
    /// <summary>
    /// Фиги инструмента
    /// </summary>
    public string Figi { get; set; }
    
    /// <summary>
    /// Интервал свечей
    /// </summary>
    public Interval Interval { get; set; }
}