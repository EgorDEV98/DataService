using System.Runtime.Serialization;

namespace DataService.Integration.Enums;

/// <summary>
/// Интервал свечи
/// </summary>
public enum CandleInterval
{
    /// <summary>
    /// 1 минута
    /// </summary>
    [EnumMember(Value = "1_MIN")]
    _1Min,
    
    /// <summary>
    /// 15 минут
    /// </summary>
    [EnumMember(Value = "15_MIN")]
    _15Min
}