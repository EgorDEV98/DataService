using DataService.Application.Interfaces;
using DataService.Application.Options;
using DataService.Contracts.Models.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

namespace DataService.Application.Workers.RealTimeWorkers;

[DisallowConcurrentExecution]
public class FifteenMinuteRealtimeCandleWorker(
    IServiceProvider serviceProvider,
    ILogger<FifteenMinuteRealtimeCandleWorker> logger,
    IOptions<CronOptions> cronOptions,
    IOptions<SessionOptions> sessionOptions,
    ICandleBufferFlusher flusher)
    : RealTimeCandleWorkerBase<FifteenMinuteRealtimeCandleWorker>(serviceProvider, logger, cronOptions, sessionOptions, flusher,
        Interval._15Min, Interval._15Min), IJob
{
    public async Task Execute(IJobExecutionContext context) => await base.Execute(context); 
}