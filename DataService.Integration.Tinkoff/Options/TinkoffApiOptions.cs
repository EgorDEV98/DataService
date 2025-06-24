namespace DataService.Integration.Tinkoff.Options;

/// <summary>
/// Настройки тинькофф интеграции
/// </summary>
public class TinkoffApiOptions
{
    /// <summary>
    /// Токен
    /// </summary>
    public string AccessToken { get; set; }
    
    /// <summary>
    /// Песочница
    /// </summary>
    public bool Sandbox { get; set; }
    
    /// <summary>
    /// Название приложения
    /// </summary>
    public string AppName { get; set; }

    public void Validate()
    {
        if(string.IsNullOrEmpty(AccessToken))
            throw new ArgumentNullException(nameof(AccessToken) + " cannot be null.");
    }
}