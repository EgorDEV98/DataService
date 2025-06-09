using DataService.Integration.Models;

namespace DataService.Integration.Interfaces;

/// <summary>
/// Провайдер подписки на стрим
/// </summary>
public interface ICandleStreamProvider
{
    /// <summary>
    /// Событие при получении новой свечи
    /// </summary>
    event Func<CandleDto, Task> OnCandleReceived;
    
    /// <summary>
    /// Запустить стрим
    /// </summary>
    /// <param name="ct">Токен</param>
    /// <returns></returns>
    Task StartAsync(CancellationToken ct = default);
    
    /// <summary>
    /// Подписаться на 
    /// </summary>
    /// <param name="subCandles">Акции для подписки</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task Subscribe(IEnumerable<SubscribeShareDto> subCandles, CancellationToken ct = default);
    
    /// <summary>
    /// Остановить стрим
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task StopAsync(CancellationToken ct = default);
}