using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Udup.Core.Roslyn.Internals;

public static class NodeExtractors
{
    public static string GetFullName(this INamedTypeSymbol @interface)
    {
        return $"{string.Join('.', @interface.ContainingNamespace.ConstituentNamespaces)}.{@interface.Name}";
    }
    
    public static bool IsUdupEvent(this INamedTypeSymbol symbol)
    {
        return symbol.Interfaces.Any(@interface =>
            @interface.GetFullName() == $"{typeof(IUdupMessage).FullName}");
    }
    
    internal static string CreateIdentifier(this ObjectCreationExpressionSyntax node)
    {
        return $"creation{node.Span.Start}e{node.Span.End}";
    }
}