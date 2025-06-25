using DataService.WebApi.Mappers;

namespace DataService.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddMappers(this IServiceCollection services)
    {
        services.AddSingleton<OrderBookMapper>();
        services.AddSingleton<ShareMapper>();
    }
}