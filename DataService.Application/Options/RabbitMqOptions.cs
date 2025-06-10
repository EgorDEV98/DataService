namespace DataService.Application.Options;

/// <summary>
/// Настройки RabbitMQ
/// </summary>
public class RabbitMqOptions
{
    /// <summary>
    /// Хост
    /// </summary>
    public string Host { get; set; }
    
    /// <summary>
    /// Пользователь
    /// </summary>
    public string Username { get; set; }
    
    /// <summary>
    /// Пароль
    /// </summary>
    public string Password { get; set; }
    
    /// <summary>
    /// V-Host
    /// </summary>
    public string VirtualHost { get; set; }
}