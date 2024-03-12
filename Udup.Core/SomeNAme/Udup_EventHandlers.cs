using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Udup.Core.SomeNAme;

public partial class UdupService
{
    public async Task<List<EventHandler>> GetEventHandlers()
    {
        var solution = await workspace.OpenSolutionAsync(@"C:\P\Edu\udup\Udup.sln");

        var list = new List<EventHandler>();
        foreach (var project in solution.Projects)
        {
            var compilation = await project.GetCompilationAsync();

            foreach (var document in project.Documents)
            {
                list.AddRange(await GetEventHandlersFromSyntaxAndSemantic(compilation, document));
            }
        }

        return list;
    }
    
    private async Task<EventHandler[]> GetEventHandlersFromSyntaxAndSemantic(Compilation? compilation, Document document)
    {
        var tree = await document.GetSyntaxTreeAsync();
        var root = tree.GetRoot();
        var semanticModel = compilation.GetSemanticModel(tree);

        return root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>()
            .Select(syntax => semanticModel.GetDeclaredSymbol(syntax))
            .Where(symbol => symbol != null)
            .Where(symbol => symbol!.IsImplicitlyDeclared is false)
            .Where(IsUdupEventHandler)
            .Select(_ => new EventHandler(_.Name, _.Interfaces.Where(@interface =>
                GetFullName(@interface) == $"{typeof(IUdupHandler).FullName}").Select(_ => _.TypeArguments.First().Name).ToArray()))
            .ToArray();
    }

    private bool IsUdupEventHandler(INamedTypeSymbol symbol)
    {
        return symbol.Interfaces
            .Where(@interface =>
                GetFullName(@interface) == $"{typeof(IUdupHandler).FullName}")
            .Where(@interface =>
                @interface.IsGenericType)
            .Any();
    }
}