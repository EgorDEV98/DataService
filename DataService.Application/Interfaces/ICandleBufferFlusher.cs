using DataService.Data.Entities;

namespace DataService.Application.Interfaces;

/// <summary>
/// Интерфейс сервиса сохранения свечей
/// </summary>
public interface ICandleBufferFlusher
{
    /// <summary>
    /// Добавить в очередь на сохранение
    /// </summary>
    /// <param name="candle">Свеча</param>
    void Enqueue(Candle candle);
    
    /// <summary>
    /// Запустить сервис
    /// </summary>
    /// <param name="ct">Токен</param>
    /// <returns></returns>
    Task StartAsync(CancellationToken ct);
    
    /// <summary>
    /// Остановить сервис
    /// </summary>
    /// <returns></returns>
    Task StopAsync();
}