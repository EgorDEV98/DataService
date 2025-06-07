using AutoFixture;

namespace DataService.Tests;

public class BaseUnitTests
{
    protected readonly Fixture Fixture;

    public BaseUnitTests()
    {
        Fixture = new Fixture();
    }
}