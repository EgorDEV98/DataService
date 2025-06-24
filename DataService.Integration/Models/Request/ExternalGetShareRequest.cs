namespace DataService.Integration.Models.Request;

/// <summary>
/// Запрос акции
/// </summary>
public class ExternalGetShareRequest
{
    /// <summary>
    /// Тикер акции
    /// </summary>
    public string Ticker { get; set; } = null!;
    
    /// <summary>
    /// Секция
    /// </summary>
    public string ClassCode { get; set; } = "TQBR";
}