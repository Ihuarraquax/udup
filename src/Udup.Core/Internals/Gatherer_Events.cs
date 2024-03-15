using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Udup.Abstractions;

namespace Udup.Core.Internals;

internal class Gatherer_Events
{
    public static async Task<IdAndName[]> Get(SyntaxTree tree, SemanticModel? semanticModel)
    {
        var root = tree.GetRoot();

        return root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>()
            .Select(syntax => semanticModel.GetDeclaredSymbol(syntax))
            .Where(symbol => symbol != null)
            .Where(symbol => symbol!.IsImplicitlyDeclared is false)
            .Where(symbol => symbol.IsUdupEvent())
            .Select(symbol => new IdAndName(symbol.Name))
            .ToArray();
    }
}