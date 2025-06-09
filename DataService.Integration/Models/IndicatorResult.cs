namespace DataService.Integration.Models;

public class IndicatorResult
{
    public string Ticker { get; set; } = string.Empty;
    public Dictionary<string, object> Indicators { get; set; } = new();
}