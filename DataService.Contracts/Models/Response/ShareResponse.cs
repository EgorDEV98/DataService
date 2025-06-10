namespace DataService.Contracts.Models.Response;

/// <summary>
/// Акция
/// </summary>
public class ShareResponse
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
}