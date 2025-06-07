using System.Runtime.Serialization;

namespace DataService.Data.Enum;

/// <summary>
/// Интервал свечи
/// </summary>
public enum CandleInterval
{
    // 15 мин
    [EnumMember(Value = "1_MIN")]
    _1Min,
    
    /// <summary>
    /// 15 мин
    /// </summary>
    [EnumMember(Value = "15_MIN")]
    _15Min,
}