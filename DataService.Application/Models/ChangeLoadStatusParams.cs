using DataService.Contracts.Models.Enums;

namespace DataService.Application.Models;

public class ChangeLoadStatusParams
{
    /// <summary>
    /// Идентификатор акции
    /// </summary>
    public required Guid Id { get; set; }
    
    /// <summary>
    /// Новый статус акции
    /// </summary>
    public required LoadStatus CandleLoadStatus { get; set; }
}