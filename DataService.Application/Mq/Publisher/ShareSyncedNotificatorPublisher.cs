using DataService.Contracts.Models.Mq;
using MassTransit;

namespace DataService.Application.Mq.Publisher;

public class ShareSyncedNotificatorPublisher(IPublishEndpoint publish)
{
    public async Task PublishAsync(ShareSynced shareSynced, CancellationToken ct)
        => await publish.Publish(shareSynced, ct);
}