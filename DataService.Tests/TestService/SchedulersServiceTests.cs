using DataService.Application.Models;
using DataService.Application.Providers;
using DataService.Application.Services;
using DataService.Data;
using DataService.Data.Entities;
using DataService.Integration.Interfaces;
using DataService.Integration.Models.Request;
using DataService.Integration.Models.Response;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DataService.Tests.TestService;

public class SchedulersServiceTests
{
    private static DbContextOptions<PostgresDbContext> CreateOptions() =>
        new DbContextOptionsBuilder<PostgresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

    [Fact]
    public async Task GetSchedulersAsync_FiltersCorrectly()
    {
        // Arrange
        var options = CreateOptions();
        var now = DateTimeOffset.UtcNow;

        var schedulers = new[]
        {
            new Scheduler
            {
                Id = Guid.NewGuid(),
                Exchange = "MOEX",
                IsTradingDay = true,
                StartTime = now,
                EndTime = now.AddHours(6)
            },
            new Scheduler
            {
                Id = Guid.NewGuid(),
                Exchange = "SPB",
                IsTradingDay = false,
                StartTime = now.AddDays(1),
                EndTime = now.AddDays(1).AddHours(6)
            }
        };

        await using (var db = new PostgresDbContext(options))
        {
            await db.Schedulers.AddRangeAsync(schedulers);
            await db.SaveChangesAsync();
        }

        await using var context = new PostgresDbContext(options);
        var service = new SchedulersService(context, null!, null!);

        // Act
        var result = await service.GetSchedulersAsync(new GetSchedulersParams
        {
            Exchange = "moex",
            IsTradingDay = true,
            StartTime = now.AddMinutes(-1),
            EndTime = now.AddHours(7)
        }, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("MOEX", result.First().Exchange);
    }
    
    [Fact]
    public async Task PreloadSchedulersAsync_HandlesEmptyProviderResponse()
    {
        // Arrange
        var options = CreateOptions();

        var providerMock = new Mock<ISchedulersProvider>();
        providerMock
            .Setup(x => x.GetTradingSchedulersAsync(It.IsAny<ExternalGetTradingSchedulers>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        await using var context = new PostgresDbContext(options);
        var service = new SchedulersService(context, new GuidProvider(), providerMock.Object);

        // Act
        var result = await service.PreloadSchedulersAsync(new PreloadSchedulerParams
        {
            Exchange = "MOEX",
            From = DateTimeOffset.UtcNow,
            To = DateTimeOffset.UtcNow.AddDays(1)
        }, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Empty(await context.Schedulers.ToListAsync());
    }
}
