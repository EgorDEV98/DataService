namespace DataService.Integration.Models.Response;

public class ExternalGetTradingSchedulersResponse
{
    /// <summary>
    /// Наименование торговой площадки
    /// </summary>
    public string Exchange { get; set; }
    
    /// <summary>
    /// Дата
    /// </summary>
    public DateTimeOffset Date { get; set; }
    
    /// <summary>
    /// Признак торгового дня на бирже
    /// </summary>
    public bool IsTradingDay { get; set; }
    
    /// <summary>
    /// Начала торгов
    /// </summary>
    public DateTimeOffset? StartTime { get; set; }
    
    /// <summary>
    /// Окончание торгов
    /// </summary>
    public DateTimeOffset? EndTime { get; set; }
}