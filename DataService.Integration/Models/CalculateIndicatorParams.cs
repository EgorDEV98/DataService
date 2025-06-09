namespace DataService.Integration.Models;

public class CalculateIndicatorParams
{
    public required string Ticker { get; set; }
    public required IEnumerable<CandleDto> Candles1Min { get; set; }
    public required IEnumerable<CandleDto> Candles15Min { get; set; }
}