using DataService.Application.Interfaces;

namespace DataService.Application.Provider;

public class GuidProvider : IGuidProvider
{
    public Guid GetGuid() => Guid.NewGuid();
}