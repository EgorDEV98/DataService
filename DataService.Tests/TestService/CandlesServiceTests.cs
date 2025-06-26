using AutoFixture;
using DataService.Application.Models;
using DataService.Application.Services;
using DataService.Contracts.Models.Enums;
using DataService.Data;
using DataService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataService.Tests.TestService;

public class CandlesServiceTests
{
    private readonly Fixture _fixture;

    public CandlesServiceTests()
    {
        _fixture = new Fixture();

        // Убираем рекурсию
        _fixture.Behaviors
            .OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    private static DbContextOptions<PostgresDbContext> CreateOptions() =>
        new DbContextOptionsBuilder<PostgresDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // 💡 каждый раз новая
            .Options;

    [Fact]
public async Task GetCandlesAsync_ReturnsFilteredAndOrderedCandles()
{
    // Arrange
    var options = CreateOptions();
    var shareId = Guid.NewGuid();
    var interval = Interval._1Min;
    var now = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);

    var validCandles = Enumerable.Range(0, 5)
        .Select(i => _fixture.Build<Candle>()
            .With(c => c.ShareId, shareId)
            .With(c => c.Interval, interval)
            .With(c => c.Time, now.AddMinutes(-i)) // -0, -1, -2, ...
            .Without(c => c.Share)
            .Create())
        .ToList();

    var otherCandles = new List<Candle>();

    // другие ShareId
    otherCandles.AddRange(_fixture.Build<Candle>()
        .With(c => c.ShareId, Guid.NewGuid())
        .With(c => c.Interval, interval)
        .With(c => c.Time, now)
        .Without(c => c.Share)
        .CreateMany(2));

    // другой Interval
    otherCandles.AddRange(_fixture.Build<Candle>()
        .With(c => c.ShareId, shareId)
        .With(c => c.Interval, Interval._5Min)
        .With(c => c.Time, now)
        .Without(c => c.Share)
        .CreateMany(2));

    await using (var db = new PostgresDbContext(options))
    {
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
        await db.Candles.AddRangeAsync(validCandles.Concat(otherCandles));
        await db.SaveChangesAsync();

        // контроль: только 5 валидных
        var all = await db.Candles.ToListAsync();
        var countValid = all.Count(x =>
            x.ShareId == shareId &&
            x.Interval == interval &&
            x.Time >= now.AddMinutes(-5) &&
            x.Time <= now.AddMinutes(1));

        Assert.Equal(5, countValid);
    }

    await using var context = new PostgresDbContext(options);
    var service = new CandlesService(context);

    var param = new GetCandlesParams
    {
        ShareId = shareId,
        Interval = interval,
        From = now.AddMinutes(-5),
        To = now.AddMinutes(1),
        Offset = 0,
        Limit = 3
    };

    // Act
    var result = await service.GetCandlesAsync(param);

    // Assert
    Assert.Equal(3, result.Count);
    Assert.All(result, x => Assert.Equal(shareId, x.ShareId));
    Assert.All(result, x => Assert.Equal(interval, x.Interval));
    Assert.True(result.SequenceEqual(result.OrderByDescending(c => c.Time)));
}


    [Fact]
    public async Task GetCandlesAsync_UsesDefaultLimit_WhenNotProvided()
    {
        // Arrange
        var options = CreateOptions();
        var interval = Interval._1Min;
        var count = 10;

        var share = _fixture.Build<Share>()
            .With(x => x.Id, Guid.NewGuid())
            .Without(x => x.Candles) // предотвращаем циклические связи
            .Create();

        var candles = _fixture.Build<Candle>()
            .With(c => c.ShareId, share.Id)
            .With(c => c.Interval, interval)
            .Without(c => c.Share) // избегаем обратной связи, чтобы EF не тянул лишнее
            .CreateMany(count)
            .ToList();

        await using (var db = new PostgresDbContext(options))
        {
            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();
            await db.Shares.AddAsync(share);
            await db.Candles.AddRangeAsync(candles);
            await db.SaveChangesAsync();

            // контроль: убеждаемся, что в базе 10 записей
            var realCount = await db.Candles.CountAsync();
            Assert.Equal(count, realCount);
        }

        await using var context = new PostgresDbContext(options);
        var service = new CandlesService(context);

        var param = new GetCandlesParams
        {
            ShareId = share.Id,
            Interval = interval
        };

        // Act
        var result = await service.GetCandlesAsync(param);

        // Assert
        Assert.Equal(count, result.Count);
    }

}
