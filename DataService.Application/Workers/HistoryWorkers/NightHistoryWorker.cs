using DataService.Application.Interfaces;
using DataService.Contracts.Models.Enums;
using DataService.Data;
using DataService.Data.Entities;
using DataService.Integration.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

namespace DataService.Application.Workers.HistoryWorkers;

public class NightHistoryWorker(
    ILogger<NightHistoryWorker> logger, 
    IServiceProvider serviceProvider, 
    IGuidProvider guidProvider) 
    : BaseHistoryWorker<NightHistoryWorker>(logger, guidProvider), IJob
{
    public static readonly JobKey JobKey = new(nameof(NightHistoryWorker));
    
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("{Worker} Start", nameof(NightHistoryWorker));
        await Run(context.CancellationToken);
        logger.LogInformation("{Worker} End", nameof(NightHistoryWorker));
    }

    private async Task Run(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var postgres = scope.ServiceProvider.GetRequiredService<PostgresDbContext>();
        var candleProvider = scope.ServiceProvider.GetRequiredService<ICandlesProvider>();

        // Получаем список акций для синхронизации
        var sharesForSync = await LoadShares(postgres, cancellationToken);

        foreach (var share in sharesForSync)
            await base.StartWork(share, postgres, candleProvider, cancellationToken);
    }

    private static async Task<IReadOnlyCollection<Share>> LoadShares(PostgresDbContext postgres, CancellationToken cancellationToken)
        => await postgres.Shares
            .AsNoTracking()
            .Where(x => x.CandleLoadStatus == LoadStatus.Enabled)
            .ToListAsync(cancellationToken);
}