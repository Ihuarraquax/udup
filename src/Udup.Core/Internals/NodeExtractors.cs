using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Udup.Abstractions;

namespace Udup.Core.Internals;

public static class NodeExtractors
{
    public static bool IsUdupEvent(this INamedTypeSymbol symbol)
    {
        return symbol.Interfaces.Any(@interface =>
            @interface.ToDisplayString() == $"{typeof(IUdupMessage).FullName}");
    }
}