namespace DataService.Contracts.Models.Request;

public class GetOrderBookByFigiRequest
{
    /// <summary>
    /// Фиги
    /// </summary>
    public required string Figi { get; set; }
    
    /// <summary>
    /// Глубина
    /// </summary>
    public required int Depth { get; set; }
}