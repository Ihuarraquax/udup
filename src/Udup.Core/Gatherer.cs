using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Udup.Abstractions;
using Udup.Core.Internals.Walkers;
using EventHandler = Udup.Abstractions.EventHandler;

namespace Udup.Core;

public class Gatherer : CSharpSyntaxVisitor, IGatherEventHandlers, IGatherEventTraces, IGatherEvents
{
    private readonly List<(SyntaxNode root, SemanticModel semanticModel)> rootsWithSemantics;

    public Gatherer(List<(SyntaxNode root, SemanticModel semanticModel)> rootsWithSemantics)
    {
        this.rootsWithSemantics = rootsWithSemantics;
    }

    public UdupResponse Gather()
    {
        var events = new List<IdAndName>();
        var eventHandlers = new List<EventHandler>();
        var eventTraces = new List<EventTrace>();

        foreach (var (root, semantic) in rootsWithSemantics)
        {
            var gatherer_events = new EventWalker(semantic);
            gatherer_events.Visit(root);
            events.AddRange(gatherer_events.Events);

            var gatherer_eventsHandlers = new EventHandlersWalker(semantic);
            gatherer_eventsHandlers.Visit(root);
            eventHandlers.AddRange(gatherer_eventsHandlers.EventHandlers);

            var gatherer_eventTraces = new EventTracesWalker(rootsWithSemantics, semantic);
            gatherer_eventTraces.Visit(root);
            eventTraces.AddRange(gatherer_eventTraces.EventTraces);
        }

        return new UdupResponse(
            events,
            eventHandlers,
            eventTraces
        );
    }

    public List<EventHandler> GatherEventHandlers()
    {
        var eventHandlers = new List<EventHandler>();
        foreach (var (root, semantic) in rootsWithSemantics)
        {
            var gatherer_eventsHandlers = new EventHandlersWalker(semantic);
            gatherer_eventsHandlers.Visit(root);
            eventHandlers.AddRange(gatherer_eventsHandlers.EventHandlers);
        }

        return eventHandlers;
    }

    public List<EventTrace> GatherEventTraces()
    {
        var eventTraces = new List<EventTrace>();
        foreach (var (root, semantic) in rootsWithSemantics)
        {
            var gatherer_eventTraces = new EventTracesWalker(rootsWithSemantics, semantic);
            gatherer_eventTraces.Visit(root);
            eventTraces.AddRange(gatherer_eventTraces.EventTraces);
        }

        return eventTraces;
    }

    public List<IdAndName> GatherEvents()
    {
        var events = new List<IdAndName>();

        foreach (var (root, semantic) in rootsWithSemantics)
        {
            var gatherer_events = new EventWalker(semantic);
            gatherer_events.Visit(root);
            events.AddRange(gatherer_events.Events);
        }

        return events;
    }
}