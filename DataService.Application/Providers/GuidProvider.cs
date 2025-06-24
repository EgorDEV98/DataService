using DataService.Application.Interfaces;

namespace DataService.Application.Providers;

public class GuidProvider : IGuidProvider
{
    public Guid GetGuid() => Guid.NewGuid();
}