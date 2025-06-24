using DataService.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DataService.Application.Extensions;

public static class ApplicationExtensions
{
    /// <summary>
    /// Предзагрузить акции в БД
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static async Task PreloadSharesAsync(this IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var shareService = scope.ServiceProvider.GetRequiredService<IShareService>();
        await shareService.PreloadSharesAsync();
    }
}