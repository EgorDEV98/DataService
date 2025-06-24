using DataService.Contracts.Models.Enums;

namespace DataService.Contracts.Models.Response;

public class GetShareResponse
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Тикер акции
    /// </summary>
    public string Ticker { get; set; } = null!;

    /// <summary>
    /// Секция торгов
    /// </summary>
    public string ClassCode { get; set; } = null!;
    
    /// <summary>
    /// Лотность
    /// </summary>
    public int Lot { get; set; }
    
    /// <summary>
    /// Валюта расчетов
    /// </summary>
    public string Currency { get; set; } = null!;
    
    /// <summary>
    /// Доступность операции в шорт
    /// </summary>
    public bool ShortEnabledFlag { get; set; }
    
    /// <summary>
    /// Имя акции
    /// </summary>
    public string Name { get; set; } = null!;
    
    /// <summary>
    /// Код страны риска — то есть страны, в которой компания ведет основной бизнес.
    /// </summary>
    public string CountryOfRisk { get; set; } = null!;
    
    /// <summary>
    /// Наименование страны риска — то есть страны, в которой компания ведет основной бизнес.
    /// </summary>
    public string CountryOfRiskName { get; set; } = null!;
    
    /// <summary>
    /// Сектор экономики.
    /// </summary>
    public string Sector { get; set; } = null!;
    
    /// <summary>
    /// Признак наличия дивидендной доходности.
    /// </summary>
    public bool DivYieldFlag { get; set; }
    
    /// <summary>
    /// Шаг цены
    /// </summary>
    public decimal MinPriceIncrement { get; set; }
    
    /// <summary>
    /// Флаг, отображающий доступность торговли инструментом по выходным.
    /// </summary>
    public bool WeekendFlag { get; set; }
    
    /// <summary>
    /// Дата первой минутной свечи
    /// </summary>
    public DateTimeOffset First1MinCandleDate { get; set; }
    
    /// <summary>
    /// Дата первой минутной свечи
    /// </summary>
    public DateTimeOffset First1DayCandleDate { get; set; }
    
    /// <summary>
    /// Статус загрузки свечей
    /// </summary>
    public LoadStatus CandleLoadStatus { get; set; }
}