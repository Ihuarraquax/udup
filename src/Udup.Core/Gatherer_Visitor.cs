using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Udup.Abstractions;
using Udup.Core.Internals;
using EventHandler = Udup.Abstractions.EventHandler;

namespace Udup.Core;

public class Gatherer_Visitor : CSharpSyntaxVisitor
{
    private readonly List<(SyntaxNode root, SemanticModel semanticModel)> rootsWithSemantics;
    private readonly SemanticModel semanticModel;

    public Gatherer_Visitor(List<(SyntaxNode root, SemanticModel semanticModel)> rootsWithSemantics)
    {
        this.rootsWithSemantics = rootsWithSemantics;
        this.semanticModel = semanticModel;
    }

    public UdupResponse Gather()
    {
        var events = new List<IdAndName>();
        var eventHandlers = new List<EventHandler>();
        
        foreach (var (root, semantic) in rootsWithSemantics)
        {
            var gatherer_events = new Gatherer_Events(semantic);
            gatherer_events.Visit(root);
            events.AddRange(gatherer_events.Events);
            
            var gatherer_eventsHandlers = new Gatherer_EventHandlers(semantic);
            gatherer_eventsHandlers.Visit(root);
            eventHandlers.AddRange(gatherer_eventsHandlers.EventHandlers);
        }
        
        var gatherer_eventTraces = new Gatherer_EventTraces(rootsWithSemantics);
        gatherer_events.Visit(root);
        events.AddRange(gatherer_events.Events);
        
        return new UdupResponse(
            events,
            eventHandlers,
            eventTraces
        );
    }
    
    public override void Visit(SyntaxNode? node)
    {
        events.Visit(node);
        eventHandlers.Visit(node);
        eventTraces.Visit(node);
    }
}