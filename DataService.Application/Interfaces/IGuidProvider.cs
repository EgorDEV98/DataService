namespace DataService.Application.Interfaces;

/// <summary>
/// Интерфейс генерации GUID
/// </summary>
public interface IGuidProvider
{
    /// <summary>
    /// Новый GUID
    /// </summary>
    /// <returns></returns>
    public Guid GetGuid();
}