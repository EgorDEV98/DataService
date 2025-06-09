using DataService.Application.Interfaces;
using DataService.Application.Options;
using DataService.Integration.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using CandleInterval = DataService.Data.Enum.CandleInterval;

namespace DataService.Application.Workers.RealTimeWorkers;

[DisallowConcurrentExecution]
public class OneMinuteRealtimeCandleWorker(
    IServiceProvider serviceProvider,
    ILogger<OneMinuteRealtimeCandleWorker> logger,
    IOptions<CronOptions> options,
    ICandleBufferFlusher flusher)
    : RealTimeCandleWorkerBase<OneMinuteRealtimeCandleWorker>(serviceProvider, logger, options,
        flusher, CandleInterval._1Min, SubscribeInterval._1Min), IJob
{
    public async Task Execute(IJobExecutionContext context) => await base.Execute(context);
}