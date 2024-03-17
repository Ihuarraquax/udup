using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Udup.Abstractions;
using EventHandler = Udup.Abstractions.EventHandler;

namespace Udup.Core.Internals;

internal class Gatherer_EventHandlers
{
    private static readonly SymbolDisplayFormat DisplayFormat =
        new(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.None);
    public static async Task<EventHandler[]> Get(SyntaxTree tree, SemanticModel? semanticModel)
    {
        var root = tree.GetRoot();

        return root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>()
            .Select(syntax => semanticModel.GetDeclaredSymbol(syntax))
            .Where(symbol => symbol != null)
            .Where(symbol => symbol!.IsImplicitlyDeclared is false)
            .Where(IsUdupEventHandler)
            .Select(_ => new EventHandler(new(_.Name), _.Interfaces
                .Where(@interface => @interface.ToDisplayString(DisplayFormat) == $"{typeof(IUdupHandler).FullName}")
                .Select(_ => new IdAndName(_.TypeArguments.First().Name))
                .ToArray()))
            .ToArray();
    }

    private static bool IsUdupEventHandler(INamedTypeSymbol symbol)
    {
        return symbol.Interfaces
            .Where(@interface => @interface.ToDisplayString(DisplayFormat) == $"{typeof(IUdupHandler).FullName}")
            .Any(@interface => @interface.IsGenericType);
    }
}