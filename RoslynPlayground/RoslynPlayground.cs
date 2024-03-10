﻿using FluentAssertions;

namespace RoslynPlayground;

public class RoslynPlayground : IClassFixture<Udup>
{
    private readonly Udup udup;

    public RoslynPlayground(Udup udup)
    {
        this.udup = udup;
    }
    
    [Fact]
    public async Task GetsAllEventNames()
    {
        // Arrange

        // Act
        var events = await udup.GetEvents();

        // Assert
        events.Should().BeEquivalentTo(
        [
            "DomainEventAHappened", 
            "DomainEventBHappened", 
            "DomainEventCHappened",
            "DomainEventDHappened"
        ]);
    }
    
    [Fact]
    public async Task GetsAllEventHandlers()
    {
        // Arrange

        // Act
        var eventHandlers = await udup.GetEventHandlers();

        // Assert
        await Verify(eventHandlers);
    }
}