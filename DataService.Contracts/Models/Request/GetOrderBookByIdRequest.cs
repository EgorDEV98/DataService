namespace DataService.Contracts.Models.Request;

public class GetOrderBookByIdRequest
{
    /// <summary>
    /// Идентификатор акции
    /// </summary>
    public required Guid Id { get; set; }
    
    /// <summary>
    /// Глубина стакана
    /// </summary>
    public required int Depth { get; set; }
}