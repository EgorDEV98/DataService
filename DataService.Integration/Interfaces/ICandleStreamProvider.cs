using DataService.Integration.Models.Request;
using DataService.Integration.Models.Response;

namespace DataService.Integration.Interfaces;

public interface ICandleStreamProvider
{
    /// <summary>
    /// Событие при получении новой свечи
    /// </summary>
    event Func<ExternalCandleResponse, Task> OnCandleReceived;
    
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
    Task Subscribe(IEnumerable<ExternalSubscribeShareRequest> subCandles, CancellationToken ct = default);
    
    /// <summary>
    /// Остановить стрим
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task StopAsync(CancellationToken ct = default);
}