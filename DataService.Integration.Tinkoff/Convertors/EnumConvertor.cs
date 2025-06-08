using Tinkoff.InvestApi.V1;
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
}