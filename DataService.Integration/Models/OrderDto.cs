namespace DataService.Integration.Models;

/// <summary>
/// Заявка
/// </summary>
public class OrderDto
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