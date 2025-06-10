namespace DataService.Contracts.Models.Response;

public class OrderBookResponse
{
    /// <summary>
    /// Figi
    /// </summary>
    public required string Figi { get; set; }
    
    /// <summary>
    /// Глубина
    /// </summary>
    public required int Depth { get; set; }
    
    /// <summary>
    /// Биды
    /// </summary>
    public List<OrderResponse> Bids { get; set; }
    
    /// <summary>
    /// Аски
    /// </summary>
    public List<OrderResponse> Asks { get; set; }
}