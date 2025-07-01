using System.Threading.Channels;
using DataService.Application.Interfaces;
using DataService.Data;
using DataService.Integration.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DataService.Application.Workers.HistoryWorkers;

public class HandleHistoryWorker(
    ILogger<HandleHistoryWorker> logger,
    IGuidProvider guidProvider,
    IServiceProvider serviceProvider) : BaseHistoryWorker<HandleHistoryWorker>(logger, guidProvider), IHostedService
{
    private readonly Channel<Guid> _channel = Channel.CreateUnbounded<Guid>();
    private readonly CancellationTokenSource _cts = new();
    private Task? _processingTask;


    public ValueTask EnqueueAsync(Guid request)
        => _channel.Writer.WriteAsync(request);

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _processingTask = Task.Run(ProcessQueueAsync, _cts.Token);
        return Task.CompletedTask;
    }
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _cts.CancelAsync();
        _channel.Writer.Complete();
        if (_processingTask != null)
            await _processingTask;
    }
    private async Task ProcessQueueAsync()
    {
        try
        {
            while (await _channel.Reader.WaitToReadAsync(_cts.Token))
            {
                while (_channel.Reader.TryRead(out var request))
                {
                    await HandleRequestAsync(request);
                }
            }
        }
        catch (OperationCanceledException ex)
        {
            logger.LogInformation(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Worker аварийно завершился");
        }
    }
    private async Task HandleRequestAsync(Guid guid)
    {
        try
        {
            await CreateScope(guid);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при обработке запроса {Guid}", guid);
        }
    }
    private async Task CreateScope(Guid guid)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<PostgresDbContext>();
        var candleProvider = scope.ServiceProvider.GetRequiredService<ICandlesProvider>();
        
        // Получаем текущую акцию
        var share = await context.Shares.FirstAsync(x => x.Id == guid, _cts.Token);
        await StartWork(share, context, candleProvider, _cts.Token);
    }
    
}