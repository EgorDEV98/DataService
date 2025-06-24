namespace DataService.Application.Interfaces;

/// <summary>
/// Создание Guid
/// </summary>
public interface IGuidProvider
{
    Guid GetGuid();
}