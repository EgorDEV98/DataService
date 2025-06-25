using DataService.Contracts.Models.Enums;

namespace DataService.Contracts.Models.Request;

public class GetSharesRequest
{
    /// <summary>
    /// Тикер
    /// </summary>
    public string? Ticker { get; set; }
    
    /// <summary>
    /// Фиги
    /// </summary>
    public string? Figi { get; set; }
    
    /// <summary>
    /// Валюта
    /// </summary>
    public string? Currency { get; set; }
    
    /// <summary>
    /// Секция
    /// </summary>
    public string? ClassCode { get; set; }
    
    /// <summary>
    /// Статус загрузки
    /// </summary>
    public LoadStatus? CandleLoadStatus { get; set; }
    
    /// <summary>
    /// Отступ
    /// </summary>
    public int? Offset { get; set; }
    
    /// <summary>
    /// Лимит
    /// </summary>
    public int? Limit { get; set; }
}