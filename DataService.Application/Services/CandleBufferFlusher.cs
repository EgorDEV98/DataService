using System.Collections.Concurrent;
using DataService.Application.Interfaces;
using DataService.Data;
using DataService.Data.Entities;
using DataService.Data.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DataService.Application.Services;

public class CandleBufferFlusher(IServiceProvider sp, ILogger<CandleBufferFlusher> logger) : ICandleBufferFlusher
{
    private readonly ConcurrentQueue<Candle> _queue = new();
    private CancellationTokenSource? _cts;
    private Task? _flushTask;

    public void Enqueue(Candle candle) => _queue.Enqueue(candle);

    public Task StartAsync(CancellationToken ct)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        _flushTask = Task.Run(() => FlushLoop(_cts.Token), _cts.Token);
        return Task.CompletedTask;
    }

    public async Task StopAsync()
    {
        if (_cts is null) return;
        await _cts.CancelAsync();

        try { if (_flushTask is not null) await _flushTask; }
        catch (Exception ex) { logger.LogWarning(ex, "Flush loop stopped with error"); }

        _cts.Dispose();
        _cts = null;
    }

    private async Task FlushLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(1000, token);
                await FlushAsync(token);
            }
            catch (TaskCanceledException) { }
            catch (Exception ex)
            {
                logger.LogError(ex, "Flush loop error");
            }
        }
    }

    private async Task FlushAsync(CancellationToken ct)
    {
        if (_queue.IsEmpty) return;

        var candles = new List<Candle>();
        while (_queue.TryDequeue(out var c)) candles.Add(c);
        if (candles.Count == 0) return;

        await using var scope = sp.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<PostgresDbContext>();
        await db.BulkInsertAsync(candles, cancellationToken: ct);

        logger.LogInformation("Добавлено {Count} свечей", candles.Count);
    }
}