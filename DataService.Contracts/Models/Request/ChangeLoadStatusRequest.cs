using DataService.Contracts.Models.Enums;

namespace DataService.Contracts.Models.Request;

public class ChangeLoadStatusRequest
{
    /// <summary>
    /// Статус
    /// </summary>
    public required LoadStatus CandleLoadStatus { get; set; }
}