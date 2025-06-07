using System.Runtime.Serialization;

namespace DataService.Data.Enum;

/// <summary>
/// Тип загрузки свечи
/// </summary>
public enum LoadType
{
    /// <summary>
    /// Ночной загрузкой
    /// </summary>
    [EnumMember(Value = "NIGHT_SYNC")]
    NightSync,
    
    /// <summary>
    /// Вебсокет загрузкой
    /// </summary>
    [EnumMember(Value = "REALTIME_SYNC")]
    RealtimeSync,
}