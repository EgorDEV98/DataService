namespace DataService.Application.Options;

/// <summary>
/// Настройки сервиса синхронизации
/// </summary>
public class SyncShareOptions
{
    /// <summary>
    /// Включение только определенных класс кодов
    /// </summary>
    public string[]? IncludesClassCode { get; set; }
    
    /// <summary>
    /// Включить для квалифицированных инвесторов
    /// </summary>
    public bool? IsQualInvestor { get; set; }
    
    /// <summary>
    /// Включить только определенные тикеры
    /// </summary>
    public string[]? IncludesTicker { get; set; }
    
    /// <summary>
    /// Включить определенные страны
    /// </summary>
    public string[]? IncludesCountryCode { get; set; }
    
    /// <summary>
    /// Включить определенные валюты
    /// </summary>
    public string[]? IncludesCurrency { get; set; }
}