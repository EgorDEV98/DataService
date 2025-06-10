using DataService.Contracts.Models.Enums;

namespace DataService.Contracts.Models.Mq;

public record NewCandle(
    string Figi,
    DateTimeOffset Time,
    Interval Interval,
    decimal Open,
    decimal High,
    decimal Low,
    decimal Close,
    decimal Volume
);