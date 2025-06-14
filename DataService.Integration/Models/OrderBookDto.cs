namespace DataService.Integration.Models;

public class OrderBookDto
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
    public List<OrderDto> Bids { get; set; }
    
    /// <summary>
    /// Аски
    /// </summary>
    public List<OrderDto> Asks { get; set; }
}