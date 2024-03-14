using Udup.Core.Roslyn;

namespace RoslynPlayground;

public class RoslynPlayground : IClassFixture<Gatherer>
{
    private readonly Gatherer gatherer;

    public RoslynPlayground(Gatherer gatherer)
    {
        this.gatherer = gatherer;
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