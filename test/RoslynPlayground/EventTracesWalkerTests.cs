using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Udup.Abstractions;
using Udup.Core.Internals.Walkers;

namespace RoslynPlayground;

public class EventTracesWalkerTests
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
        await Verify(result);
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
        await Verify(result);
    }

    [Fact]
    public async Task _()
    {
        // Arrange
        var tree = CSharpSyntaxTree.ParseText(@"
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Udup.Abstractions;
using Udup.WebApp.EF;

namespace Udup.WebApp;

public record DomainEventXHappened : INotification, IUdupMessage;

public static class Endpoints2
{
    public static void MapDomainEventBEndpointsWithService2(this WebApplication app)
    {
        app.MapGet(""/domainEventX"",
            ([FromServices] IMediator mediator) => { mediator.Publish(new DomainEventXHappened()); });

        app.MapGet(""/domainEventXService"",
                ([FromServices] IMediator mediator, IDomainEventXService service) => service.SendXEvent())
            .WithOpenApi();

        app.MapPost(""/domainEventXDomain"",
                ([FromServices] IMediator mediator, IDomainEventBService service) =>
                {
                    var sample = new Sample();
                    sample.MakeActionX();
                })
            .WithOpenApi();
    }
}

public interface IDomainEventXService
{
    Task SendXEvent();
}

class DomainEventXService : IDomainEventXService
{
    private readonly IMediator mediator;

    public DomainEventXService(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public Task SendXEvent()
    {
        return mediator.Send(new DomainEventXHappened());
    }
}

public class XActioner
{
    public Guid Id { get; set; }
    
    private List<INotification> domainEvents;

    public IReadOnlyCollection<INotification> DomainEvents => domainEvents?.AsReadOnly();

    public void AddDomainEvent(INotification eventItem)
    {
        domainEvents.Add(eventItem);
    }
    
    public void MakeActionX()
    {
        AddDomainEvent(new DomainEventXHappened());
    }
}

");
        // Act
        var result = await Act(tree);

        // Assert
        await Verify(result);
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

        var gatherer = new EventTracesWalker(new List<(SyntaxNode root, SemanticModel semanticModel)>()
        {
            (await tree.GetRootAsync(), model)
        }, model);
        
        gatherer.Visit(await tree.GetRootAsync());
        return gatherer.EventTraces;
    }

    #endregion
}