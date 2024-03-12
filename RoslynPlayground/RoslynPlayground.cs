
using Udup.Core.SomeNAme;

namespace RoslynPlayground;

public class RoslynPlayground : IClassFixture<UdupService>
{
    private readonly UdupService udupService;

    public RoslynPlayground(UdupService udupService)
    {
        this.udupService = udupService;
    }
    
    [Fact]
    public async Task GetsAllEventNames()
    {
        // Arrange

        // Act
        var events = await udupService.GetEvents();

        // Assert
        await Verify(events);
    }
    
    [Fact]
    public async Task GetsAllEventHandlers()
    {
        // Arrange

        // Act
        var eventHandlers = await udupService.GetEventHandlers();

        // Assert
        await Verify(eventHandlers);
    }
}