using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Udup;
using EventHandler = Udup.EventHandler;

namespace RoslynPlayground;

public class Udup
{
    private readonly MSBuildWorkspace workspace;

    public Udup()
    {
        MSBuildLocator.RegisterDefaults();
        
        this.workspace = MSBuildWorkspace.Create();
        this.workspace.LoadMetadataForReferencedProjects = true;

        this.workspace.WorkspaceFailed += (sender, args) => { Console.WriteLine(args.Diagnostic.Message); };
    }
    
    public async Task<List<string>> GetEvents()
    {
        var solution = await workspace.OpenSolutionAsync(@"C:\P\Edu\udup\Udup.sln");
        var events = new List<string>();
        foreach (var project in solution.Projects)
        {
            var compilation = await project.GetCompilationAsync();

            foreach (var document in project.Documents)
            {
                events.AddRange(await GetEventsFromSyntaxAndSemantic(compilation, document));
            }
        }

        return events;
    }

    private async Task<string[]> GetEventsFromSyntaxAndSemantic(Compilation? compilation, Document document)
    {
        var tree = await document.GetSyntaxTreeAsync();
        var root = tree.GetRoot();
        var semanticModel = compilation.GetSemanticModel(tree);

        return root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>()
            .Select(syntax => semanticModel.GetDeclaredSymbol(syntax))
            .Where(symbol => symbol != null)
            .Where(symbol => symbol!.IsImplicitlyDeclared is false)
            .Where(IsUdupEvent)
            .Select(symbol => symbol.Name)
            .ToArray();
    }

    private static bool IsUdupEvent(INamedTypeSymbol symbol)
    {
        return symbol.Interfaces.Any(@interface =>
            $"{@interface.ContainingNamespace.Name}.{@interface.Name}" == $"{typeof(IUdupMessage).FullName}");
    }

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
                $"{@interface.ContainingNamespace.Name}.{@interface.Name}" == $"{typeof(IUdupHandler).FullName}").Select(_ => _.TypeArguments.First().Name).ToArray()))
            .ToArray();
    }

    private bool IsUdupEventHandler(INamedTypeSymbol symbol)
    {
        return symbol.Interfaces
            .Where(@interface =>
                $"{@interface.ContainingNamespace.Name}.{@interface.Name}" == $"{typeof(IUdupHandler).FullName}")
            .Where(@interface =>
                @interface.IsGenericType)
            .Any();
    }
}