namespace DataService.Integration.Models;

/// <summary>
/// ДТО акции
/// </summary>
public class ShareDto
{
    /// <summary>
    /// FIGI-идентификатор инструмента.
    /// </summary>
    public string Figi { get; init; } = null!;
    
    /// <summary>
    /// Тикер инструмента.
    /// </summary>
    public string Ticker { get; init; } = null!;
    
    /// <summary>
    /// Код страны
    /// </summary>
    public string CountryOfRisk { get; init; } = null!;
    
    /// <summary>
    /// Секуция торгов
    /// </summary>
    public string ClassCode { get; init; } = null!;
    
    /// <summary>
    /// Для квалифицированных инвесторов
    /// </summary>
    public bool ForQualInvestor { get; init; }
    
    /// <summary>
    /// Валюта расчетов
    /// </summary>
    public string Currency { get; init; } = null!;
    
    /// <summary>
    /// Название инструмента
    /// </summary>
    public string Name { get; init; } = null!;
    
    /// <summary>
    /// Первая минутная свеча
    /// </summary>
    public DateTimeOffset First1MinCandleDate { get; set; }
    
    /// <summary>
    /// Первая дневная свеча
    /// </summary>
    public DateTimeOffset First1DayCandleDate { get; set; }
}