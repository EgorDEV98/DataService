using AutoFixture;
using DataService.Application.Interfaces;
using DataService.Application.Models;
using DataService.Application.Providers;
using DataService.Application.Services;
using DataService.Contracts.Models.Enums;
using DataService.Data;
using DataService.Data.Entities;
using DataService.Integration.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DataService.Tests.TestService;

public class SharesServiceTests
{
    private readonly PostgresDbContext _dbContext;
    private readonly SharesService _service;
    private readonly Fixture _fixture = new();
    private readonly Mock<ISharesProvider> _sharesProviderMock = new();
    private readonly Mock<IGuidProvider> _guidProviderMock = new();

    public SharesServiceTests()
    {
        var options = new DbContextOptionsBuilder<PostgresDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new PostgresDbContext(options);

        _fixture.Behaviors
            .OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _service = new SharesService(_dbContext, _guidProviderMock.Object, _sharesProviderMock.Object, null);
    }

    [Fact]
    public async Task GetShareAsync_ReturnsShare_WhenExists()
    {
        // Arrange
        var share = _fixture.Build<Share>().With(x => x.Id, Guid.NewGuid()).Create();
        await _dbContext.Shares.AddAsync(share);
        await _dbContext.SaveChangesAsync();

        var param = new GetShareParams { Id = share.Id };

        // Act
        var result = await _service.GetShareAsync(param);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(share.Id, result.Id);
    }

    [Fact]
    public async Task GetShareAsync_Throws_WhenNotFound()
    {
        var param = new GetShareParams { Id = Guid.NewGuid() };

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetShareAsync(param));
    }

    [Fact]
    public async Task GetSharesAsync_ReturnsFiltered()
    {
        // Arrange
        var share1 = _fixture.Build<Share>().With(x => x.Ticker, "SBER").With(x => x.CandleLoadStatus, LoadStatus.Enabled).Create();
        var share2 = _fixture.Build<Share>().With(x => x.CandleLoadStatus, LoadStatus.Disabled).Create();
        await _dbContext.Shares.AddRangeAsync(share1, share2);
        await _dbContext.SaveChangesAsync();

        var param = new GetSharesParams { Ticker = "SBER" };

        // Act
        var result = await _service.GetSharesAsync(param);

        // Assert
        Assert.Single(result);
        Assert.Equal("SBER", result.First().Ticker);
    }

    [Fact]
    public async Task ChangeLoadStatus_ChangesStatus_ById()
    {
        var share = _fixture.Build<Share>()
            .With(x => x.Id, Guid.NewGuid())
            .With(x => x.CandleLoadStatus, LoadStatus.Disabled)
            .Create();

        await _dbContext.Shares.AddAsync(share);
        await _dbContext.SaveChangesAsync();

        var param = new ChangeLoadStatusParams
        {
            Id = share.Id,
            CandleLoadStatus = LoadStatus.Enabled
        };

        // Act
        var result = await _service.ChangeLoadStatus(param);

        // Assert
        var updated = await _dbContext.Shares.FindAsync(share.Id);
        Assert.True(result);
        Assert.Equal(LoadStatus.Enabled, updated.CandleLoadStatus);
    }

    [Fact]
    public async Task ChangeLoadStatus_Throws_WhenShareNotFound()
    {
        var param = new ChangeLoadStatusParams
        {
            Id = Guid.NewGuid(),
            CandleLoadStatus = LoadStatus.Disabled
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.ChangeLoadStatus(param));
    }
}