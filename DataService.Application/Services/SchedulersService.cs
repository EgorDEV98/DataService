using DataService.Application.Interfaces;
using DataService.Application.Models;
using DataService.Data;
using DataService.Data.Entities;
using DataService.Data.Extensions;
using DataService.Integration.Interfaces;
using DataService.Integration.Models.Request;
using Microsoft.EntityFrameworkCore;

namespace DataService.Application.Services;

public class SchedulersService(PostgresDbContext context, IGuidProvider guidProvider, ISchedulersProvider schedulersProvider) : ISchedulersService
{
    public async Task<IReadOnlyCollection<Scheduler>> GetSchedulersAsync(GetSchedulersParams param, CancellationToken cancellationToken)
        => await context.Schedulers
            .WhereIf(!string.IsNullOrWhiteSpace(param.Exchange), x => x.Exchange == param.Exchange)
            .WhereIf(param.IsTradingDay.HasValue, x => x.IsTradingDay == param.IsTradingDay)
            .WhereIf(param.StartTime.HasValue, x => x.StartTime >= param.StartTime)
            .WhereIf(param.EndTime.HasValue, x => x.EndTime <= param.EndTime)
            .ToArrayAsync(cancellationToken);

    public async Task<bool> PreloadSchedulersAsync(PreloadSchedulerParams param, CancellationToken cancellationToken)
    {
        var externalSchedulers = (await schedulersProvider.GetTradingSchedulersAsync(new ExternalGetTradingSchedulers()
        {
            Exchange = param.Exchange,
            From = param.From,
            To = param.To,
        }, cancellationToken))
        .Select(x => new Scheduler()
        {
            Id = guidProvider.GetGuid(),
            Exchange = x.Exchange,
            IsTradingDay = x.IsTradingDay,
            StartTime = x.StartTime,
            EndTime = x.EndTime
        });
        await context.BulkInsertAsync(externalSchedulers, cancellationToken: cancellationToken);
        return true;
    }
}