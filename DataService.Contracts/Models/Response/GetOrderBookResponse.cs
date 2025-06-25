namespace DataService.Contracts.Models.Response;

public class GetOrderBookResponse
{
    /// <summary>
    /// Figi
    /// </summary>
    public string Figi { get; set; }
    
    /// <summary>
    /// Глубина
    /// </summary>
    public int Depth { get; set; }
    
    /// <summary>
    /// Биды
    /// </summary>
    public List<OrderResponse> Bids { get; set; }
    
    /// <summary>
    /// Аски
    /// </summary>
    public List<OrderResponse> Asks { get; set; }
}