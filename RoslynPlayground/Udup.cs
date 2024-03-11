using System.Text;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Extensions.Primitives;
using Udup;
using EventHandler = Udup.EventHandler;

namespace RoslynPlayground;

public partial class Udup
{
    private readonly MSBuildWorkspace workspace;

    public Udup()
    {
        MSBuildLocator.RegisterDefaults();

        this.workspace = MSBuildWorkspace.Create();
        this.workspace.LoadMetadataForReferencedProjects = true;

        this.workspace.WorkspaceFailed += (sender, args) => { Console.WriteLine(args.Diagnostic.Message); };
    }

    public async Task<List<EventWithTrace>> GetEvents()
    {
        var solution = await workspace.OpenSolutionAsync(@"C:\P\Edu\udup\Udup.sln");
        var events = new List<string>();
        var eventSources = new List<(string Event, string Source)>();
        foreach (var project in solution.Projects)
        {
            var compilation = await project.GetCompilationAsync();

            foreach (var document in project.Documents)
            {
                events.AddRange(await GetEventsFromSyntaxAndSemantic(compilation, document));
                eventSources.AddRange(await GetEventSource(compilation, document));
            }
        }

        var eventSourcesDictionary = eventSources
            .GroupBy(_ => _.Event)
            .ToDictionary(
                _ => _.Key,
                _ => _
                    .Select(_ => _.Source)
                    .ToList());

        return events.Select(e => new EventWithTrace
        {
            Name = e,
            Sources = eventSourcesDictionary.TryGetValue(e, out var value) ? value : []
        }).ToList();
    }

    private async Task<List<(string Event, string Source)>> GetEventSource(Compilation? compilation, Document document)
    {
        var tree = await document.GetSyntaxTreeAsync();
        var root = await tree.GetRootAsync();
        var semanticModel = compilation.GetSemanticModel(tree);

        var result = root.DescendantNodes().OfType<ObjectCreationExpressionSyntax>()
            .Where(_ => IsUdupEvent(semanticModel.GetSymbolInfo(_).Symbol.ContainingType))
            .ToArray();

        var list = new List<(string Event, string Source)>();
        foreach (var node in result)
        {
            var stringBuilder = BuildPath(node, semanticModel);

            list.Add((semanticModel.GetSymbolInfo(node).Symbol.ContainingType.Name, stringBuilder.ToString()));
        }

        return list;
    }

    private static string BuildPath(SyntaxNode node, SemanticModel semanticModel)
    {
        var stringBuilder = new StringBuilder();

        if (node is null)
        {
            return "";
        }

        stringBuilder.Append(BuildPath(node.Parent, semanticModel));

        if (node is MethodDeclarationSyntax methodDeclaration)
        {
            stringBuilder.Append(methodDeclaration.Identifier.Text);
            stringBuilder.Append("()");
        }

        if (node is BaseTypeDeclarationSyntax typeDeclaration)
        {
            stringBuilder.Append(typeDeclaration.Identifier.Text);
            stringBuilder.Append(".");
        }
        
        if (node is ExpressionStatementSyntax expression)
        {
            stringBuilder.Append("Endpoint (TODO - gather more details)");
        }

        return stringBuilder.ToString();
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
}

public class EventWithTrace
{
    public string Name { get; set; }
    public List<string> Sources { get; set; }
}