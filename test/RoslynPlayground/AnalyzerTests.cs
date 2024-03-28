using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Udup.Abstractions;
using Udup.Analyzer;

namespace RoslynPlayground;

public class AnalyzerTests
{
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


    private static async Task<List<UdupType>> Act(SyntaxTree tree)
    {
        var Mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        var udup = MetadataReference.CreateFromFile(typeof(IUdupMessage).Assembly.Location);
        var WebApplication = MetadataReference.CreateFromFile(typeof(WebApplication).Assembly.Location);
        var compilation = CSharpCompilation.Create("MyCompilation",
            syntaxTrees: new[] { tree },
            references: new[] { Mscorlib, udup, WebApplication });
        var model = compilation.GetSemanticModel(tree);

        return new Analyzer().Analyze([(await tree.GetRootAsync(), model)]);
    }
}