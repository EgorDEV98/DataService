using System.Runtime.Serialization;

namespace DataService.Contracts.Models.Enums;

/// <summary>
/// Статус загрузки
/// </summary>
public enum LoadStatus
{
    /// <summary>
    /// Исключено из загрузки
    /// </summary>
    [EnumMember(Value = "DISABLED")]
    Disabled,
    
    /// <summary>
    /// Включено в загрузку
    /// </summary>
    [EnumMember(Value = "ENABLE")]
    Enabled,
}