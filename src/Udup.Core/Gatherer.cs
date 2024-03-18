using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using NuGet.Packaging;
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
                var semanticModel = compilation.GetSemanticModel(tree);

                var eventHandlerWalker = new Gatherer_EventHandlers(semanticModel);
                
                events.AddRange(await Gatherer_Events.Get(tree, semanticModel));
                eventHandlers.AddRange(eventHandlerWalker.GetWithWalker(tree));
                eventTraces.AddRange(await Gatherer_EventTraces.Get(tree, semanticModel));
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
                events.AddRange(await Gatherer_Events.Get(tree, semanticModel));
            });

        return events;
    }

    public async Task<List<EventTrace>> GatherEventTraces()
    {
        var solution = await OpenSolutionAsync();

        var eventTraces = new List<EventTrace>();
        
        await IterateThroughProjectsAndDocuments(solution,
            async (tree, semanticModel) =>
            {
                eventTraces.AddRange(await Gatherer_EventTraces.Get(tree, semanticModel));
            });
        
        return eventTraces;
    }

    public async Task<List<EventHandler>> GatherEventHandlers()
    {
        var solution = await OpenSolutionAsync();

        var eventHandlers = new List<EventHandler>();

        foreach (var project in solution.Projects)
        {
            var compilation = await project.GetCompilationAsync();

            foreach (var document in project.Documents)
            {
                var tree = await document.GetSyntaxTreeAsync();
                var semanticModel = compilation.GetSemanticModel(tree);

                var walker = new Gatherer_EventHandlers(semanticModel);
                walker.Visit(await tree.GetRootAsync());
                eventHandlers.AddRange(walker.eventHandlers);
            }
        }
        return eventHandlers;
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
}