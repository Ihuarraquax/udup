using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;
using Udup.Core.Roslyn.Internals;

namespace Udup.Core.Roslyn;

public class Gatherer
{
    private readonly MSBuildWorkspace workspace;

    public Gatherer()
    {
        MSBuildLocator.RegisterDefaults();

        workspace = MSBuildWorkspace.Create();
        workspace.LoadMetadataForReferencedProjects = true;

        workspace.WorkspaceFailed += (sender, args) => { Console.WriteLine(args.Diagnostic.Message); };
    }


    public async Task<UdupResponse> Gather()
    {
        var solution = await workspace.OpenSolutionAsync(@"C:\P\Edu\udup\Udup.sln");

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
        var solution = await workspace.OpenSolutionAsync(@"C:\P\Edu\udup\Udup.sln");

        return await Gatherer_Events.GatherEvents(solution);
    }
    
    public async Task<List<EventTrace>> GatherEventTraces()
    {
        var solution = await workspace.OpenSolutionAsync(@"C:\P\Edu\udup\Udup.sln");

        return await Gatherer_EventTraces.GatherEventsTraces(solution);
    }

    public async Task<List<EventHandler>> GatherEventHandlers()
    {
        var solution = await workspace.OpenSolutionAsync(@"C:\P\Edu\udup\Udup.sln");

        return await Gatherer_EventHandlers.GetEventHandlers(solution);
    }
}