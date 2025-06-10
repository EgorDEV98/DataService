namespace DataService.Application.Models;

/// <summary>
/// Модель получения книги
/// </summary>
public class GetOrderBookParams
{
    /// <summary>
    /// Идентификатор акции
    /// </summary>
    public required Guid ShareId { get; set; }
    
    /// <summary>
    /// Глубина
    /// </summary>
    public required int Depth { get; set; }
}