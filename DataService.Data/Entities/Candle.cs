using DataService.Data.Enum;

namespace DataService.Data.Entities;

/// <summary>
/// Свеча
/// </summary>
public class Candle
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; init; }
    
    /// <summary>
    /// Цена открытия
    /// </summary>
    public double Open { get; init; }
    
    /// <summary>
    /// Верхняя цена
    /// </summary>
    public double High { get; init; }
    
    /// <summary>
    /// Нижняя цена
    /// </summary>
    public double Low { get; init; }
    
    /// <summary>
    /// Цена закрытия
    /// </summary>
    public double Close { get; init; }
    
    /// <summary>
    /// Объем
    /// </summary>
    public long Volume { get; init; }
    
    /// <summary>
    /// Время свечи
    /// </summary>
    public DateTimeOffset Time { get; init; }
    
    /// <summary>
    /// Интервал свечи
    /// </summary>
    public CandleInterval Interval { get; init; }
    
    /// <summary>
    /// Тип загрузки
    /// </summary>
    public LoadType LoadType { get; init; }

    /// <summary>
    /// Навигационное поле
    /// </summary>
    public Share Share { get; init; } = null!;
    
    /// <summary>
    /// Внешний ключ
    /// </summary>
    public Guid ShareId { get; init; }
}