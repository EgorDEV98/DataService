using DataService.Application.Interfaces;
using DataService.Application.Options;
using DataService.Integration.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using CandleInterval = DataService.Data.Enum.CandleInterval;

namespace DataService.Application.Workers.RealTimeWorkers;

[DisallowConcurrentExecution]
public class FifteenMinuteRealtimeCandleWorker(
    IServiceProvider serviceProvider,
    ILogger<FifteenMinuteRealtimeCandleWorker> logger,
    IOptions<CronOptions> options,
    ICandleBufferFlusher flusher)
    : RealTimeCandleWorkerBase<FifteenMinuteRealtimeCandleWorker>(serviceProvider, logger, options, flusher,
        CandleInterval._15Min, SubscribeInterval._15Min), IJob
{
    public async Task Execute(IJobExecutionContext context) => await base.Execute(context); 
}