using DataService.Application.Providers;

namespace DataService.Tests.TestCommon;

public class GuidProviderTests
{
    [Fact]
    public void GetGuid_ReturnsNonEmptyGuid()
    {
        // Arrange
        var provider = new GuidProvider();

        // Act
        var guid = provider.GetGuid();

        // Assert
        Assert.NotEqual(Guid.Empty, guid);
    }

    [Fact]
    public void GetGuid_ReturnsUniqueGuids()
    {
        // Arrange
        var provider = new GuidProvider();

        // Act
        var guid1 = provider.GetGuid();
        var guid2 = provider.GetGuid();

        // Assert
        Assert.NotEqual(guid1, guid2);
    }
}