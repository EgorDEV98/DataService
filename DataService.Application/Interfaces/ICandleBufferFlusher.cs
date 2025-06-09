using DataService.Data.Entities;

namespace DataService.Application.Interfaces;

public interface ICandleBufferFlusher
{
    void Enqueue(Candle candle);
    Task StartAsync(CancellationToken ct);
    Task StopAsync();
}