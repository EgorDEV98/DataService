using DataService.Integration.Interfaces;
using DataService.Integration.Models.Request;
using DataService.Integration.Models.Response;
using DataService.Integration.Tinkoff.Common;
using Google.Protobuf.WellKnownTypes;
using Tinkoff.InvestApi.V1;

namespace DataService.Integration.Tinkoff.Providers;

public class SchedulersProvider(InstrumentsService.InstrumentsServiceClient client, InstrumentRateLimiter limiter) : ISchedulersProvider
{
    public async Task<IReadOnlyCollection<ExternalGetTradingSchedulersResponse>> GetTradingSchedulersAsync(ExternalGetTradingSchedulers request, CancellationToken cancellationToken)
    {
        await limiter.WaitAsync(cancellationToken);
        return (await client.TradingSchedulesAsync(new TradingSchedulesRequest()
            {
                Exchange = request.Exchange ?? string.Empty,
                From = request.From.HasValue ? Timestamp.FromDateTimeOffset(request.From.Value) : null,
                To = request.To.HasValue ? Timestamp.FromDateTimeOffset(request.To.Value) : null
            })).Exchanges
            .SelectMany(exchange => exchange.Days.Select(day => new ExternalGetTradingSchedulersResponse
            {
                Exchange = exchange.Exchange,
                IsTradingDay = day.IsTradingDay,
                StartTime = day.StartTime?.ToDateTimeOffset(),
                EndTime = day.EndTime?.ToDateTimeOffset()
            }))
            .ToList();
    }
}