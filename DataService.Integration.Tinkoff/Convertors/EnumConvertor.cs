using DataService.Integration.Enums;
using Tinkoff.InvestApi.V1;
using CandleInterval = Tinkoff.InvestApi.V1.CandleInterval;
using IntegrationInterval = DataService.Integration.Enums.CandleInterval;

namespace DataService.Integration.Tinkoff.Convertors;

public static class EnumConvertor
{
    public static CandleInterval Convert(this IntegrationInterval candleInterval)
        => candleInterval switch
        {
            IntegrationInterval._1Min => CandleInterval._1Min,
            IntegrationInterval._15Min => CandleInterval._15Min,
            _ => throw new ArgumentOutOfRangeException(nameof(candleInterval), candleInterval, null)
        };
    
    public static SubscriptionInterval Convert(this SubscribeInterval interval)
        => interval switch
        {
            SubscribeInterval._1Min => SubscriptionInterval.OneMinute,
            SubscribeInterval._15Min => SubscriptionInterval.FifteenMinutes,
            _ => throw new ArgumentOutOfRangeException(nameof(interval), interval, null)
        };
}