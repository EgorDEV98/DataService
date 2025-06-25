using AutoFixture;
using DataService.Application.Models;
using DataService.Application.Services;
using DataService.Data;
using DataService.Data.Entities;
using DataService.Integration.Interfaces;
using DataService.Integration.Models.Request;
using DataService.Integration.Models.Response;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DataService.Tests.TestService
{
    public class OrderBookServiceTests
    {
        private readonly Fixture _fixture;

        public OrderBookServiceTests()
        {
            _fixture = new Fixture();
        }
        
        private static DbContextOptions<PostgresDbContext> CreateOptions()
        {
            return new DbContextOptionsBuilder<PostgresDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task GetOrderBookByFigiAsync_ReturnsOrderBook()
        {
            // Arrange
            var response = new ExternalGetOrderBookResponse { Figi = "FIGI123" };

            var providerMock = new Mock<IOrderBookProvider>();
            providerMock
                .Setup(p => p.GetOrderBookAsync(It.Is<ExternalGetOrderBookRequest>(r =>
                    r.Figi == "FIGI123" && r.Depth == 10), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var service = new OrderBookService(null!, providerMock.Object);

            var result = await service.GetOrderBookByFigiAsync(new GetOrderBookParams
            {
                Figi = "FIGI123",
                Depth = 10
            });

            // Assert
            Assert.Equal("FIGI123", result.Figi);
        }

        [Fact]
        public async Task GetOrderBookByIdAsync_ReturnsOrderBook()
        {
            // Arrange
            var dbOptions = CreateOptions();
            var shareId = Guid.NewGuid();

            _fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));

            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var share = _fixture.Build<Share>()
                .With(x => x.Id, shareId)
                .With(x => x.Figi, "FIGI456")
                .Create();

            await using (var dbContext = new PostgresDbContext(dbOptions))
            {
                await dbContext.Shares.AddAsync(share);
                await dbContext.SaveChangesAsync();
            }

            var providerMock = new Mock<IOrderBookProvider>();
            providerMock
                .Setup(p => p.GetOrderBookAsync(It.Is<ExternalGetOrderBookRequest>(r =>
                    r.Figi == "FIGI456" && r.Depth == 5), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExternalGetOrderBookResponse { Figi = "FIGI456" });

            await using var context = new PostgresDbContext(dbOptions);
            var service = new OrderBookService(context, providerMock.Object);

            // Act
            var result = await service.GetOrderBookByIdAsync(new GetOrderBookByIdParams
            {
                Id = shareId,
                Depth = 5
            });

            // Assert
            Assert.Equal("FIGI456", result.Figi);
        }


        [Fact]
        public async Task GetOrderBookByIdAsync_ThrowsIfShareNotFound()
        {
            // Arrange
            var dbOptions = CreateOptions();
            var providerMock = new Mock<IOrderBookProvider>();

            await using var context = new PostgresDbContext(dbOptions);
            var service = new OrderBookService(context, providerMock.Object);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                service.GetOrderBookByIdAsync(new GetOrderBookByIdParams
                {
                    Id = Guid.NewGuid(),
                    Depth = 5
                }));

            Assert.Contains("No order book found with id", ex.Message);
        }
    }
}
