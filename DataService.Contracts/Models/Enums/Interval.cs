using System.Runtime.Serialization;

namespace DataService.Contracts.Models.Enums;

/// <summary>
/// Интервал свечи
/// </summary>
public enum Interval
{
    /// <summary>
    /// 1 минута
    /// </summary>
    [EnumMember(Value = "CANDLE_INTERVAL_1_MIN")]
    _1Min,
    
    /// <summary>
    /// 2 минуты
    /// </summary>
    [EnumMember(Value = "CANDLE_INTERVAL_2_MIN")]
    _2Min,
    
    /// <summary>
    /// 3 минуты
    /// </summary>
    [EnumMember(Value = "CANDLE_INTERVAL_3_MIN")]
    _3Min,
    
    /// <summary>
    /// 5 минут
    /// </summary>
    [EnumMember(Value = "CANDLE_INTERVAL_5_MIN")]
    _5Min,
    
    /// <summary>
    /// 10 минут
    /// </summary>
    [EnumMember(Value = "CANDLE_INTERVAL_10_MIN")]
    _10Min,
    
    /// <summary>
    /// 15 минут
    /// </summary>
    [EnumMember(Value = "CANDLE_INTERVAL_15_MIN")]
    _15Min,
    
    /// <summary>
    /// 30 минут
    /// </summary>
    [EnumMember(Value = "CANDLE_INTERVAL_30_MIN")]
    _30Min,
    
    /// <summary>
    /// 1 час
    /// </summary>
    [EnumMember(Value = "CANDLE_INTERVAL_HOUR")]
    _1Hour,
    
    /// <summary>
    /// 2 часа
    /// </summary>
    [EnumMember(Value = "CANDLE_INTERVAL_2_HOUR")]
    _2Hour,
    
    /// <summary>
    /// 4 часа
    /// </summary>
    [EnumMember(Value = "CANDLE_INTERVAL_4_HOUR")]
    _4Hour,
    
    /// <summary>
    /// 1 день
    /// </summary>
    [EnumMember(Value = "CANDLE_INTERVAL_DAY")]
    _1Day,
    
    /// <summary>
    /// 1 неделя
    /// </summary>
    [EnumMember(Value = "CANDLE_INTERVAL_WEEK")]
    _1Week,
    
    /// <summary>
    /// 1 месяц
    /// </summary>
    [EnumMember(Value = "CANDLE_INTERVAL_MONTH")]
    _1Month
}