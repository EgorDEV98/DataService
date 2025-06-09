namespace DataService.Integration.Tinkoff.Options;

/// <summary>
/// Настройки тинькофф апи
/// </summary>
public class TinkoffOptions
{
    /// <summary>
    /// Токен
    /// </summary>
    public string AccessToken { get; set; } = null!;
    
    /// <summary>
    /// Имя приложения
    /// </summary>
    public string? AppName { get; set; } = nameof(DataService);

    /// <summary>
    /// Режим песочницы
    /// </summary>
    public bool Sandbox { get; set; }

    /// <summary>
    /// Валидация полей
    /// </summary>
    public void Validate()
    {
        if(string.IsNullOrWhiteSpace(AccessToken))
            throw new ArgumentNullException(nameof(AccessToken));
    }
}