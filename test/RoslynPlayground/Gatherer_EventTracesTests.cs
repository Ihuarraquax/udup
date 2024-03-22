using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Udup.Abstractions;
using Udup.Core.Internals;

namespace RoslynPlayground;

public class Gatherer_EventTracesTests
{
    [Fact]
    public async Task MethodInClass()
    {
        // Arrange
        var tree = CSharpSyntaxTree.ParseText(@"
using MediatR;
using Udup.Abstractions;

namespace Udup.WebApp;

public record DomainEventBHappened : INotification, IUdupMessage;

public interface IDomainEventBService
{
    Task SendBEvent();
}

class DomainEventBService : IDomainEventBService
{
    private readonly IMediator mediator;

    public DomainEventBService(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public Task SendBEvent()
    {
        return mediator.Send(new DomainEventBHappened());
    }
}
");
        // Act
        var result = await Act(tree);

        // Assert
        var eventTrace = result.Should().ContainSingle().Which;
        eventTrace.Event.Name.Should().Be("DomainEventBHappened");
        eventTrace.Trace.Name.Should().Be("DomainEventBService.SendBEvent()");
    }

    [Fact]
    public async Task InAppMapGet()
    {
        // Arrange
        var tree = CSharpSyntaxTree.ParseText(@"
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Udup.Abstractions;

namespace Udup.WebApp;

public record DomainEventAHappened : INotification, IUdupMessage;

public static class Endpoints
{
    public static void MapDomainEventBEndpointsWithService(this WebApplication app)
    {
        app.MapGet(""/domainEventA"", ([FromServices] IMediator mediator) =>
        {
            mediator.Publish(new DomainEventAHappened());
        });
    }
}
");
        // Act
        var result = await Act(tree);

        // Assert
        var eventTrace = result.Should().ContainSingle().Which;
        eventTrace.Event.Name.Should().Be("DomainEventAHappened");
        eventTrace.Trace.Name.Should().Be("GET \"/domainEventA\"");
    }

    #region Arrange

    private static async Task<List<EventTrace>> Act(SyntaxTree tree)
    {
        var Mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        var udup = MetadataReference.CreateFromFile(typeof(IUdupMessage).Assembly.Location);
        var WebApplication = MetadataReference.CreateFromFile(typeof(WebApplication).Assembly.Location);
        var compilation = CSharpCompilation.Create("MyCompilation",
            syntaxTrees: new[] { tree }, references: new[] { Mscorlib, udup, WebApplication });
        var model = compilation.GetSemanticModel(tree);

        var gatherer = new Gatherer_EventTraces(model);
        gatherer.Visit(await tree.GetRootAsync());
        return gatherer.EventTraces;
    }

    #endregion
}