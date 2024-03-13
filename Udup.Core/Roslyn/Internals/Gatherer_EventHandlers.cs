using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Udup.Core.Roslyn.Internals;

internal class Gatherer_EventHandlers
{
    internal static async Task<List<EventHandler>> GetEventHandlers(Solution solution)
    {
        var list = new List<EventHandler>();
        foreach (var project in solution.Projects)
        {
            var compilation = await project.GetCompilationAsync();

            foreach (var document in project.Documents)
            {
                list.AddRange(await GatherFromDocument(compilation, document));
            }
        }

        return list;
    }

    private static async Task<EventHandler[]> GatherFromDocument(Compilation? compilation,
        Document document)
    {
        var tree = await document.GetSyntaxTreeAsync();
        var root = tree.GetRoot();
        var semanticModel = compilation.GetSemanticModel(tree);

        return root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>()
            .Select(syntax => semanticModel.GetDeclaredSymbol(syntax))
            .Where(symbol => symbol != null)
            .Where(symbol => symbol!.IsImplicitlyDeclared is false)
            .Where(IsUdupEventHandler)
            .Select(_ => new EventHandler(new(_.Name), _.Interfaces.Where(@interface => @interface.GetFullName() == $"{typeof(IUdupHandler).FullName}")
                .Select(_ => new IdAndName(_.TypeArguments.First().Name))
                .ToArray()))
            .ToArray();
    }

    private static bool IsUdupEventHandler(INamedTypeSymbol symbol)
    {
        return symbol.Interfaces
            .Where(@interface => @interface.GetFullName() == $"{typeof(IUdupHandler).FullName}")
            .Any(@interface => @interface.IsGenericType);
    }
}