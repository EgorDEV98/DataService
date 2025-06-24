using DataService.Data.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataService.Data;

/// <summary>
/// Точка входа для слоя БД
/// </summary>
public static class Entry
{
    /// <summary>
    /// Добавить слой базы данных
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddPostgres(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var postgresOptions = configuration
            .GetSection(nameof(PostgresOptions))
            .Get<PostgresOptions>() ?? throw new ArgumentException(nameof(PostgresOptions) + " is not configured");
        postgresOptions.Validate();
        
        serviceCollection.AddDbContext<PostgresDbContext>(o =>
        {
            o.UseNpgsql(postgresOptions.ConnectionString);
        });
        
        return serviceCollection;
    }
}