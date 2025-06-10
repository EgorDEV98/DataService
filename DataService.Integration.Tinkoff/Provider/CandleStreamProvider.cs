using AutoMapper;
using DataService.Contracts.Models.Enums;
using DataService.Integration.Interfaces;
using DataService.Integration.Models;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Tinkoff.InvestApi;
using Tinkoff.InvestApi.V1;

namespace DataService.Integration.Tinkoff.Provider;

/// <summary>
/// Реализация потока
/// </summary>
public class CandleStreamProvider(ILogger<CandleStreamProvider> logger, InvestApiClient client, IMapper mapper) : ICandleStreamProvider
{
    public event Func<CandleDto, Task>? OnCandleReceived;
    
    private AsyncDuplexStreamingCall<MarketDataRequest, MarketDataResponse>? _stream;
    private CancellationTokenSource? _cts;
    
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
                    var realtimeCandle = mapper.Map<CandleDto>(response.Candle);

                    if (OnCandleReceived is not null)
                        await OnCandleReceived.Invoke(realtimeCandle);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[RealtimeStream] Ошибка в потоке данных");
                throw;
            }
        }, _cts.Token);
    }

    public async Task Subscribe(IEnumerable<SubscribeShareDto> subShares, CancellationToken ct = default)
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

            await _stream.RequestStream.WriteAsync(request, ct);
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
            Interval._15Min => SubscriptionInterval.FifteenMinutes,
            _ => throw new ArgumentOutOfRangeException(nameof(interval), interval, null)
        };
}