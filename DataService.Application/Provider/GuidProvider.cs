using DataService.Application.Interfaces;

namespace DataService.Application.Provider;

/// <summary>
/// Реализация получения Guid
/// </summary>
public class GuidProvider : IGuidProvider
{
    /// <summary>
    /// Новый GUID
    /// </summary>
    /// <returns></returns>
    public Guid GetGuid() => Guid.NewGuid();
}