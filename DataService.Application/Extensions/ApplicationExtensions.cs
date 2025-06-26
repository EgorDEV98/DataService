using DataService.Application.Interfaces;
using DataService.Application.Models;
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

    /// <summary>
    /// Предзагрузка режима работы торгов
    /// </summary>
    /// <param name="serviceProvider"></param>
    public static async Task PreloadSchedulersAsync(this IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var schedulerService = scope.ServiceProvider.GetRequiredService<ISchedulersService>();
        await schedulerService.PreloadSchedulersAsync(new PreloadSchedulerParams()
        {
            Exchange = "MOEX"
        }, CancellationToken.None);
    }
}