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
        var source = @"
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
                    var actioner = new XActioner();
                    actioner.MakeActionX();
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

";
        // Act
        var result = await Act(source);

        // Assert
        await Verify(result);
    }


    private static async Task<List<UdupType>> Act(string source)
    {

        ProjectId projectId = ProjectId.CreateNewId();
        DocumentId documentId = DocumentId.CreateNewId(projectId);

        Solution solution = new AdhocWorkspace().CurrentSolution
            .AddProject(projectId, "Project1", "Project1", LanguageNames.CSharp)
            .AddMetadataReference(projectId, MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            .AddMetadataReference(projectId, MetadataReference.CreateFromFile(typeof(IUdupMessage).Assembly.Location))
            .AddMetadataReference(projectId, MetadataReference.CreateFromFile(typeof(WebApplication).Assembly.Location))
            .AddDocument(documentId, "File1.cs", source)
            .WithProjectCompilationOptions(projectId,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        
        Project project = solution.GetProject(projectId);
        Document document = project.GetDocument(documentId);

        var compilation = await project.GetCompilationAsync();
        var root = await document.GetSyntaxRootAsync();
        var semanticModel = compilation.GetSemanticModel(root.SyntaxTree);

        return new Analyzer(solution).Analyze([(root, semanticModel)]);
    }
}