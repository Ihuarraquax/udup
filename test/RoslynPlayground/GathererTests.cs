using Udup.Core;

namespace RoslynPlayground;

public class GathererTests : IClassFixture<GathererFixture>
{
    private readonly Gatherer gatherer;

    public GathererTests(GathererFixture gathererFixture)
    {
        this.gatherer = gathererFixture.Instance;
    }

    [Fact]
    public async Task GetsAll()
    {
        // Arrange

        // Act
        var udupResponse = await gatherer.Gather();

        // Assert
        await Verify(udupResponse);
    }
    
    [Fact]
    public async Task GetsAllEvents()
    {
        // Arrange

        // Act
        var events = await gatherer.GatherEvents();

        // Assert
        await Verify(events);
    }
    
    [Fact]
    public async Task GetsAllEventTraces()
    {
        // Arrange

        // Act
        var events = await gatherer.GatherEventTraces();

        // Assert
        await Verify(events);
    }

    [Fact]
    public async Task GetsAllEventHandlers()
    {
        // Arrange

        // Act
        var eventHandlers = await gatherer.GatherEventHandlers();

        // Assert
        await Verify(eventHandlers);
    }
}

public class GathererFixture
{
    public readonly Gatherer Instance = new(@"C:\P\Edu\udup\Udup.sln");
}