using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Udup.Abstractions;
using Udup.Core.Internals;
using EventHandler = Udup.Abstractions.EventHandler;

namespace Udup.Core;

public class Gatherer
{
    private readonly string solutionPath;
    private readonly MSBuildWorkspace workspace;

    public Gatherer(string solutionPath)
    {
        this.solutionPath = solutionPath;
        MSBuildLocator.RegisterDefaults();

        workspace = MSBuildWorkspace.Create();
        workspace.LoadMetadataForReferencedProjects = true;

        workspace.WorkspaceFailed += (sender, args) => { Console.WriteLine(args.Diagnostic.Message); };
    }
    
    public async Task<UdupResponse> Gather()
    {
        var solution = await OpenSolutionAsync();

        var events = new List<IdAndName>();
        var eventHandlers = new List<EventHandler>();
        var eventTraces = new List<EventTrace>();

        foreach (var project in solution.Projects)
        {
            var compilation = await project.GetCompilationAsync();

            foreach (var document in project.Documents)
            {
                var tree = await document.GetSyntaxTreeAsync();
                var root = await tree.GetRootAsync();
                
                var semanticModel = compilation.GetSemanticModel(tree);

                var visitor = new Gatherer_Visitor(semanticModel);
                visitor.Visit(root);
                events.AddRange(visitor.events.Events);
                eventHandlers.AddRange(visitor.eventHandlers.EventHandlers);
                eventTraces.AddRange(visitor.eventTraces.EventTraces);
            }
        }

        return new UdupResponse(
            events,
            eventHandlers,
            eventTraces
        );
    }

    public async Task<List<IdAndName>> GatherEvents()
    {
        var solution = await OpenSolutionAsync();

        var events = new List<IdAndName>();

        await IterateThroughProjectsAndDocuments(solution,
            async (tree, semanticModel) =>
            {
                var root = await tree.GetRootAsync();
                var eventWalker = new Gatherer_Events(semanticModel);
                eventWalker.Visit(root);
                events.AddRange(eventWalker.Events);
            });

        return events;
    }

    public async Task<List<EventHandler>> GatherEventHandlers()
    {
        var solution = await OpenSolutionAsync();

        var eventTraces = new List<EventHandler>();
        
        await IterateThroughProjectsAndDocuments(solution,
            async (tree, semanticModel) =>
            {
                var eventHandlerWalker = new Gatherer_EventHandlers(semanticModel);
                eventHandlerWalker.Visit(await tree.GetRootAsync());
                eventTraces.AddRange(eventHandlerWalker.EventHandlers);
            });
        
        return eventTraces;
    }

    private async Task<Solution> OpenSolutionAsync()
    {
        return await workspace.OpenSolutionAsync(solutionPath);
    }
    
    private async Task IterateThroughProjectsAndDocuments(Solution solution, Func<SyntaxTree, SemanticModel, Task> func)
    {
        foreach (var project in solution.Projects)
        {
            var compilation = await project.GetCompilationAsync();

            foreach (var document in project.Documents)
            {
                var tree = await document.GetSyntaxTreeAsync();
                var semanticModel = compilation.GetSemanticModel(tree);

                await func.Invoke(tree, semanticModel);
            }
        }
    }
    public async Task<List<EventTrace>> GatherEventTraces()
    {
        var solution = await OpenSolutionAsync();

        var eventTraces = new List<EventTrace>();
        
        await IterateThroughProjectsAndDocuments(solution,
            async (tree, semanticModel) =>
            {
                var eventTracesWalker = new Gatherer_EventTraces(semanticModel);
                eventTracesWalker.Visit(await tree.GetRootAsync());
                eventTraces.AddRange(eventTracesWalker.EventTraces);
            });
        
        return eventTraces;
    }
}