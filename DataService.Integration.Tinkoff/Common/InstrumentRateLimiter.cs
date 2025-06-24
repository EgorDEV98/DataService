namespace DataService.Integration.Tinkoff.Common;

public class InstrumentRateLimiter() : BaseRateLimiter<InstrumentRateLimiter>(200, 200, 500) { }