namespace DataService.Contracts.Models.Response;

public class OrderResponse
{
    /// <summary>
    /// Цена
    /// </summary>
    public decimal Price { get; set; }   
    
    /// <summary>
    /// Кол-во лотов
    /// </summary>
    public decimal Quantity { get; set; }
}