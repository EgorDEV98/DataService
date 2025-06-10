namespace DataService.Contracts.Models.Mq;

/// <summary>
/// Модель уведомления о полной синхронизации свечи
/// </summary>
public record ShareSynced
{
    /// <summary>
    /// Guid
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Тикер
    /// </summary>
    public string Ticker { get; set; }
}