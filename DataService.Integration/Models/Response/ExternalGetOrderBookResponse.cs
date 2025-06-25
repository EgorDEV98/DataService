namespace DataService.Integration.Models.Response;

public class ExternalGetOrderBookResponse
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
    public List<ExternalOrderDto> Bids { get; set; }
    
    /// <summary>
    /// Аски
    /// </summary>
    public List<ExternalOrderDto> Asks { get; set; }
}