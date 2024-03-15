using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Udup.Abstractions;
using Udup.Core.Internals;

namespace RoslynPlayground;

public class Gatherer_EventTracesTests
{
    [Fact]
    public async Task GetsAllEventTraces()
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

    #region Arrange

    private static async Task<List<EventTrace>> Act(SyntaxTree tree)
    {
        var Mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        var udup = MetadataReference.CreateFromFile(typeof(IUdupMessage).Assembly.Location);
        var compilation = CSharpCompilation.Create("MyCompilation",
            syntaxTrees: new[] { tree }, references: new[] { Mscorlib, udup });
        var model = compilation.GetSemanticModel(tree);

        return await Gatherer_EventTraces.Get(tree, model);
    }

    #endregion
}