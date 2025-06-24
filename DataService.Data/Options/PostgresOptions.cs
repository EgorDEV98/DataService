namespace DataService.Data.Options;

/// <summary>
/// Параметры БД
/// </summary>
public class PostgresOptions
{
    /// <summary>
    /// Строка подключения
    /// </summary>
    public string ConnectionString { get; set; } = null!;

    public void Validate()
    {
        if(string.IsNullOrEmpty(ConnectionString))
            throw new ApplicationException("Connection string is empty");
    }
}