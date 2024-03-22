using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Udup.Core.Internals;

namespace Udup.Core;

public class Gatherer_Visitor : CSharpSyntaxVisitor
{
    private readonly SemanticModel semanticModel;
    public readonly Gatherer_Events events;
    public readonly Gatherer_EventHandlers eventHandlers;
    public readonly Gatherer_EventTraces eventTraces;

    public Gatherer_Visitor(SemanticModel semanticModel)
    {
        this.semanticModel = semanticModel;
        events = new Gatherer_Events(this.semanticModel);
        eventHandlers = new Gatherer_EventHandlers(this.semanticModel);
        eventTraces = new Gatherer_EventTraces(this.semanticModel);
    }
    
    public override void Visit(SyntaxNode? node)
    {
        events.Visit(node);
        eventHandlers.Visit(node);
        eventTraces.Visit(node);
    }
}