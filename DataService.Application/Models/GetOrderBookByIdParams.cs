namespace DataService.Application.Models;

public class GetOrderBookByIdParams
{
    /// <summary>
    /// Идентификатор акции
    /// </summary>
    public required Guid Id { get; set; }
    
    /// <summary>
    /// Глубина
    /// </summary>
    public required int Depth { get; set; }
}