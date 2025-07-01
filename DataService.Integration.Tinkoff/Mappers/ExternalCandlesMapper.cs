using DataService.Contracts.Models.Enums;
using DataService.Integration.Models.Response;
using Riok.Mapperly.Abstractions;
using Tinkoff.InvestApi.V1;

namespace DataService.Integration.Tinkoff.Mappers;

[Mapper]
public partial class ExternalCandlesMapper
{
    public partial IReadOnlyCollection<ExternalCandleResponse> Map(IEnumerable<HistoricCandle> request);
    public partial ExternalCandleResponse Map(Candle request);
    
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
    public CandleInterval Map(Interval interval)
        => interval switch
        {
            Interval._1Min => CandleInterval._1Min,
            Interval._2Min => CandleInterval._2Min,
            Interval._3Min => CandleInterval._3Min,
            Interval._5Min => CandleInterval._5Min,
            Interval._10Min => CandleInterval._10Min,
            Interval._15Min => CandleInterval._15Min,
            Interval._30Min => CandleInterval._30Min,
            Interval._1Hour => CandleInterval.Hour,
            Interval._2Hour => CandleInterval._2Hour,
            Interval._4Hour => CandleInterval._4Hour,
            Interval._1Day => CandleInterval.Day,
            Interval._1Week => CandleInterval.Week,
            Interval._1Month => CandleInterval.Month,
            _ => throw new ArgumentOutOfRangeException(nameof(interval), interval, null)
        };
}