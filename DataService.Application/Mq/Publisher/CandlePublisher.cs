using DataService.Contracts.Models.Mq;
using MassTransit;

namespace DataService.Application.Mq.Publisher;

public class CandlePublisher(IPublishEndpoint publish)
{
    public async Task PublishAsync(NewCandle candle, CancellationToken ct)
        => await publish.Publish(candle, ct);
}