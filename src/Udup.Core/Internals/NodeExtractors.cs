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
    
    internal static string CreateIdentifier(this ObjectCreationExpressionSyntax? node)
    {
        return $"creation{node.Span.Start}e{node.Span.End}";
    }    
    
    internal static bool IsWebApplication(this SymbolInfo node)
    {
        if(node.Symbol is IParameterSymbol parameter)
        {
            return parameter.Type.ToDisplayString() == "Microsoft.AspNetCore.Builder.WebApplication";
        }

        return false;
    }
    
    internal static string ToHttpMethod(this string text)
    {
        return text switch
        {
            "MapGet" => "GET",
            "MapPost" => "POST",
            "MapPut" => "PUT",
            "MapDelete" => "DELETE",
            "MapPatch" => "PATCH",
            _ => "UNKNOWN"
        };
    }
}