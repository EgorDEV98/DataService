namespace DataService.Integration.Models.Response;

public class ExternalOrderDto
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