namespace DataService.Integration.Tinkoff.Common;

/// <summary>
/// Лимитер для MarketData
/// </summary>
public class MarketDataRateLimiter() : BaseRateLimiter<MarketDataRateLimiter>(600, 600, 100) { }