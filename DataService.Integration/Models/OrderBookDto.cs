namespace DataService.Integration.Models;

public class OrderBookDto
{
    /// <summary>
    /// Figi
    /// </summary>
    public required string Figi { get; set; }
    
    /// <summary>
    /// Биды
    /// </summary>
    public required decimal BidVol { get; set; }
    
    /// <summary>
    /// Аски
    /// </summary>
    public required decimal AskVol { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public required decimal BidImbalance { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public required decimal AskImbalance { get; set; }
    
    public required decimal SpreadBps { get; set; }
}