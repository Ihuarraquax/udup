using Udup.Core;

namespace RoslynPlayground;

public class GathererTests : IClassFixture<GathererFixture>
{
    private readonly SolutionReader solutionReader;

    public GathererTests(GathererFixture gathererFixture)
    {
        solutionReader = gathererFixture.Instance;
    }

    [Fact]
    public async Task GetsAll()
    {
        // Arrange

        // Act
        var udupResponse = new Gatherer(await solutionReader.GatherSolutionNodesWithSemantics()).Gather();

        // Assert
        await Verify(udupResponse);
    }
    
    [Fact]
    public async Task GetsAllEvents()
    {
        // Arrange

        // Act
        var events = new Gatherer(await solutionReader.GatherSolutionNodesWithSemantics()).GatherEvents();

        // Assert
        await Verify(events);
    }
    
    [Fact]
    public async Task GetsAllEventTraces()
    {
        // Arrange

        // Act
        var eventTraces = new Gatherer(await solutionReader.GatherSolutionNodesWithSemantics()).GatherEventTraces();

        // Assert
        await Verify(eventTraces);
    }

    [Fact]
    public async Task GetsAllEventHandlers()
    {
        // Arrange

        // Act
        var eventHandlers = new Gatherer(await solutionReader.GatherSolutionNodesWithSemantics()).GatherEventHandlers();

        // Assert
        await Verify(eventHandlers);
    }
}

public class GathererFixture
{
    public readonly SolutionReader Instance = new(@"C:\P\Edu\udup\Udup.sln");
}