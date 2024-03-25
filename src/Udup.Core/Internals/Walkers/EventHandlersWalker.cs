using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Udup.Abstractions;
using EventHandler = Udup.Abstractions.EventHandler;

namespace Udup.Core.Internals.Walkers;

public class EventHandlersWalker : CSharpSyntaxWalker
{
    private readonly SemanticModel? semanticModel;
    
    public List<EventHandler> EventHandlers = new();

    private static readonly SymbolDisplayFormat DisplayFormat =
        new(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.None);

    public EventHandlersWalker(SemanticModel? semanticModel)
    {
        this.semanticModel = semanticModel;
    }

    private static bool IsUdupEventHandler(INamedTypeSymbol symbol)
    {
        return symbol.Interfaces
            .Where(@interface => @interface.ToDisplayString(DisplayFormat) == $"{typeof(IUdupHandler).FullName}")
            .Any(@interface => @interface.IsGenericType);
    }
    
    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        var symbol = semanticModel.GetDeclaredSymbol(node);

        if (symbol == null)
            return;

        if (symbol.IsImplicitlyDeclared)
            return;

        if (!IsUdupEventHandler(symbol))
            return;

        var handler = new EventHandler(
            new(symbol.Name),
            symbol.Interfaces
                .Where(@interface => @interface.ToDisplayString(DisplayFormat) == $"{typeof(IUdupHandler).FullName}")
                .Select(_ => new IdAndName(_.TypeArguments.First().Name))
                .ToArray()
        );
        
        EventHandlers.Add(handler);
    }
}