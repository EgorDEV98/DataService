using DataService.Contracts.Models.Enums;

namespace DataService.Contracts.Models.Request;

public class GetCandlesRequest
{
    /// <summary>
    /// Идентификатор акции
    /// </summary>
    public required Guid ShareId { get; set; }
    
    /// <summary>
    /// Интервал
    /// </summary>
    public required Interval Interval { get; set; }
    
    /// <summary>
    /// С какого числа
    /// </summary>
    public DateTimeOffset? From { get; set; }
    
    /// <summary>
    /// До какого числа
    /// </summary>
    public DateTimeOffset? To { get; set; }

    /// <summary>
    /// Отступ
    /// </summary>
    public int? Offset { get; set; }
    
    /// <summary>
    /// Лимит
    /// </summary>
    public int? Limit { get; set; }
}