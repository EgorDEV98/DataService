namespace DataService.Integration.Models.Request;

public class ExternalGetOrderBookRequest
{
    private int _depth;
    /// <summary>
    /// Фиги инструмента
    /// </summary>
    public string Figi { get; set; }

    /// <summary>
    /// Глубина стакана
    /// </summary>
    public int Depth
    {
        get => _depth;
        set
        {
            if (value < 1) _depth = 10;
            else if (value >= 50) _depth = 50;
            else _depth = value;
        }
    }
}