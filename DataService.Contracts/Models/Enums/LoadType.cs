using System.Runtime.Serialization;

namespace DataService.Contracts.Models.Enums;

/// <summary>
/// Способ, по которому свеча была загружена
/// </summary>
public enum LoadType
{
    /// <summary>
    /// Воркером загрузки исторических свечей
    /// </summary>
    [EnumMember(Value = "HISTORY_WORKER")]
    HistoryWorker,
    
    /// <summary>
    /// Воркером загрузки реального времени
    /// </summary>
    [EnumMember(Value = "REALTIME_WORKER")]
    RealtimeWorker,
}