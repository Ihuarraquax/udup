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

        var events = await Gatherer_Events.GatherEvents(solution);
        var eventHandlers = await Gatherer_EventHandlers.GetEventHandlers(solution);
        var eventTraces = await Gatherer_EventTraces.GatherEventsTraces(solution);

        return new UdupResponse(
            events,
            eventHandlers,
            eventTraces
        );
    }
    
    public async Task<List<IdAndName>> GatherEvents()
    {
        var solution = await OpenSolutionAsync();

        return await Gatherer_Events.GatherEvents(solution);
    }

    public async Task<List<EventTrace>> GatherEventTraces()
    {
        var solution = await OpenSolutionAsync();

        return await Gatherer_EventTraces.GatherEventsTraces(solution);
    }

    public async Task<List<EventHandler>> GatherEventHandlers()
    {
        var solution = await OpenSolutionAsync();

        return await Gatherer_EventHandlers.GetEventHandlers(solution);
    }
    
    private async Task<Solution> OpenSolutionAsync()
    {
        return await workspace.OpenSolutionAsync(solutionPath);
    }
}