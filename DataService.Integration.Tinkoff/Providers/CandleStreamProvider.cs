using DataService.Contracts.Models.Enums;
using DataService.Integration.Interfaces;
using DataService.Integration.Models.Request;
using DataService.Integration.Models.Response;
using DataService.Integration.Tinkoff.Mappers;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tinkoff.InvestApi;
using Tinkoff.InvestApi.V1;

namespace DataService.Integration.Tinkoff.Providers;

public class CandleStreamProvider(ILogger<CandleStreamProvider> logger, InvestApiClient client, ExternalCandlesMapper mapper) : ICandleStreamProvider
{
    public event Func<ExternalCandleResponse, Task>? OnCandleReceived;
    
    private AsyncDuplexStreamingCall<MarketDataRequest, MarketDataResponse>? _stream;
    private CancellationTokenSource? _cts;
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
    
    public async Task StartAsync(CancellationToken ct = default)
    {
        logger.LogInformation("[RealtimeStream] Старт потока Tinkoff");
        _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);

        _stream = client.MarketDataStream.MarketDataStream(cancellationToken: _cts.Token);

        _ = Task.Run(async () =>
        {
            try
            {
                await foreach (var response in _stream.ResponseStream.ReadAllAsync(_cts.Token))
                {
                    if (response.PayloadCase != MarketDataResponse.PayloadOneofCase.Candle) continue;

                    if (OnCandleReceived is not null)
                    {
                        var mappedCandle = mapper.Map(response.Candle);
                        await OnCandleReceived.Invoke(mappedCandle);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[RealtimeStream] Ошибка в потоке данных");
                throw;
            }
        }, _cts.Token);
    }

    public async Task Subscribe(IEnumerable<ExternalSubscribeShareRequest> subShares, CancellationToken ct = default)
    {
        if (_stream?.RequestStream is null)
            throw new InvalidOperationException("Stream not started");

        foreach (var subscribeShare in subShares)
        {
            var request = new MarketDataRequest
            {
                SubscribeCandlesRequest = new SubscribeCandlesRequest
                {
                    SubscriptionAction = SubscriptionAction.Subscribe,
                    WaitingClose = true,
                    Instruments =
                    {
                        new CandleInstrument
                        {
                            Interval = Convert(subscribeShare.Interval),
                            InstrumentId = subscribeShare.Figi
                        }
                    }
                }
            };

            await _semaphoreSlim.WaitAsync(ct);
            await _stream.RequestStream.WriteAsync(request, ct);
            _semaphoreSlim.Release();
            
            logger.LogInformation("[RealtimeStream] Подписка на FIGI={Figi}", subscribeShare.Figi);
        }
    }

    public async Task StopAsync(CancellationToken ct = default)
    {
        logger.LogInformation("[RealtimeStream] Остановка потока Tinkoff");

        try
        {
            await _cts?.CancelAsync()!;
            if (_stream != null)
                await _stream.RequestStream.CompleteAsync();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "[RealtimeStream] Ошибка при остановке");
        }

        _cts = null;
        _stream = null;
    }

    private SubscriptionInterval Convert(Interval interval)
        => interval switch
        {
            Interval._1Min => SubscriptionInterval.OneMinute,
            Interval._2Min => SubscriptionInterval._2Min,
            Interval._3Min => SubscriptionInterval._3Min,
            Interval._5Min => SubscriptionInterval.FiveMinutes,
            Interval._10Min => SubscriptionInterval._10Min,
            Interval._15Min => SubscriptionInterval.FifteenMinutes,
            Interval._30Min => SubscriptionInterval._30Min,
            Interval._1Hour => SubscriptionInterval.OneHour,
            Interval._2Hour => SubscriptionInterval._2Hour,
            Interval._4Hour => SubscriptionInterval._4Hour,
            Interval._1Day => SubscriptionInterval.OneDay,
            Interval._1Week => SubscriptionInterval.Week,
            Interval._1Month => SubscriptionInterval.Month,
            _ => throw new ArgumentOutOfRangeException(nameof(interval), interval, null)
        };
    
    private Interval Convert(SubscriptionInterval interval)
        => interval switch
        {
            SubscriptionInterval.OneMinute => Interval._1Min,
            SubscriptionInterval._2Min => Interval._2Min,
            SubscriptionInterval._3Min => Interval._3Min,
            SubscriptionInterval.FiveMinutes => Interval._5Min,
            SubscriptionInterval._10Min => Interval._10Min,
            SubscriptionInterval.FifteenMinutes => Interval._15Min,
            SubscriptionInterval._30Min => Interval._30Min,
            SubscriptionInterval.OneHour => Interval._1Hour,
            SubscriptionInterval._2Hour => Interval._2Hour,
            SubscriptionInterval._4Hour => Interval._4Hour,
            SubscriptionInterval.OneDay => Interval._1Day,
            SubscriptionInterval.Week => Interval._1Week,
            SubscriptionInterval.Month => Interval._1Month,
            _ => throw new ArgumentOutOfRangeException(nameof(interval), interval, null)
        };
}