namespace DataService.Data.Entities;

/// <summary>
/// Акция
/// </summary>
public class Share
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }   
    
    /// <summary>
    /// FIGI-идентификатор инструмента.
    /// </summary>
    public string Figi { get; set; } = null!;
    
    /// <summary>
    /// Тикер инструмента.
    /// </summary>
    public string Ticker { get; set; } = null!;
    
    /// <summary>
    /// Название инструмента
    /// </summary>
    public string Name { get; set; } = null!;
    
    /// <summary>
    /// Признак включения в загрузку
    /// </summary>
    public bool IsEnableToLoad { get; set; }
    
    /// <summary>
    /// Первая минутная свеча
    /// </summary>
    public DateTimeOffset First1MinCandleDate { get; set; }
    
    /// <summary>
    /// Первая дневная свеча
    /// </summary>
    public DateTimeOffset First1DayCandleDate { get; set; }
    
    /// <summary>
    /// Свечи
    /// </summary>
    public ICollection<Candle>? Candles { get; set; }
}