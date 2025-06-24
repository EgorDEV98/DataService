using DataService.Integration.Interfaces;
using DataService.Integration.Models.Request;
using DataService.Integration.Models.Response;
using DataService.Integration.Tinkoff.Common;
using DataService.Integration.Tinkoff.Mappers;
using Tinkoff.InvestApi.V1;

namespace DataService.Integration.Tinkoff.Providers;

public class SharesProvider(InstrumentsService.InstrumentsServiceClient client, InstrumentRateLimiter limiter, ExternalShareMapper mapper) : ISharesProvider
{
    public async Task<IReadOnlyCollection<ExternalGetShareResponse>> GetSharesAsync(CancellationToken cancellationToken = default)
    {
        await limiter.WaitAsync(cancellationToken);
        var sharesResponse = await client.SharesAsync(cancellationToken: cancellationToken);
        var shares = sharesResponse.Instruments;
        return mapper.Map(shares);
    }

    public async Task<ExternalGetShareResponse> GetShareByTickerAsync(ExternalGetShareRequest request, CancellationToken cancellationToken = default)
    {
        await limiter.WaitAsync(cancellationToken);
        var shareResponse = await client
            .ShareByAsync(new InstrumentRequest()
            {
                IdType = InstrumentIdType.Ticker,
                ClassCode = request.ClassCode,
                Id = request.Ticker
            }, cancellationToken: cancellationToken);
        var share = shareResponse.Instrument;
        return mapper.Map(share);
    }
}