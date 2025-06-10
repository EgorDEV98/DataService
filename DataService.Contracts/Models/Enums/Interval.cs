using System.Runtime.Serialization;

namespace DataService.Contracts.Models.Enums;

/// <summary>
/// Интервал свечи
/// </summary>
public enum Interval
{
    /// <summary>
    /// 1 Минута
    /// </summary>
    [EnumMember(Value = "1_MIN")]
    _1Min,
    
    /// <summary>
    /// 15 минут
    /// </summary>
    [EnumMember(Value = "15_MIN")]
    _15Min
}