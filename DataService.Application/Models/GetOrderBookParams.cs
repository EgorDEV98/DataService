namespace DataService.Application.Models;

public class GetOrderBookParams
{
    /// <summary>
    /// Фиги акции
    /// </summary>
    public required string Figi { get; set; }
    
    /// <summary>
    /// Глубина стакана
    /// </summary>
    public required int Depth { get; set; }
}