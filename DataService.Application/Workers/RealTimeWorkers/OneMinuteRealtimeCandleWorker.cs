using DataService.Application.Interfaces;
using DataService.Application.Options;
using DataService.Contracts.Models.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

namespace DataService.Application.Workers.RealTimeWorkers;

[DisallowConcurrentExecution]
public class OneMinuteRealtimeCandleWorker(
    IServiceProvider serviceProvider,
    ILogger<OneMinuteRealtimeCandleWorker> logger,
    IOptions<CronOptions> cronOptions,
    IOptions<SessionOptions> sessionOptions,
    ICandleBufferFlusher flusher)
    : RealTimeCandleWorkerBase<OneMinuteRealtimeCandleWorker>(serviceProvider, logger, cronOptions, sessionOptions,
        flusher, Interval._1Min, Interval._1Min), IJob
{
    public async Task Execute(IJobExecutionContext context) => await base.Execute(context);
}