namespace DataService.Integration.Models.Response;

public class ExternalCandleResponse
{
    /// <summary>
    /// Цена открытия
    /// </summary>
    public decimal Open { get; init; }
    
    /// <summary>
    /// Верхняя цена
    /// </summary>
    public decimal High { get; init; }
    
    /// <summary>
    /// Нижняя цена
    /// </summary>
    public decimal Low { get; init; }
    
    /// <summary>
    /// Цена закрытия
    /// </summary>
    public decimal Close { get; init; }
    
    /// <summary>
    /// Объем
    /// </summary>
    public long Volume { get; init; }
    
    /// <summary>
    /// Время свечи
    /// </summary>
    public DateTimeOffset Time { get; init; }
}