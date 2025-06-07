namespace DataService.Data.Options;

/// <summary>
/// Настройки БД
/// </summary>
public class PostgresOptions
{
    /// <summary>
    /// Строка подключения
    /// </summary>
    public string ConnectionString { get; init; } = null!;

    public void Validate()
    {
        if(string.IsNullOrEmpty(ConnectionString))
            throw new ArgumentException(nameof(ConnectionString) + " is not configured");
    }
}