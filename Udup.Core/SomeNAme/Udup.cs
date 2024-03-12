using System.Text;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

namespace Udup.Core.SomeNAme;

public partial class UdupService
{
    private readonly MSBuildWorkspace workspace;

    public UdupService()
    {
        MSBuildLocator.RegisterDefaults();

        workspace = MSBuildWorkspace.Create();
        workspace.LoadMetadataForReferencedProjects = true;

        workspace.WorkspaceFailed += (sender, args) => { Console.WriteLine(args.Diagnostic.Message); };
    }


    public async Task<UdupResponse> Get()
    {
        var events = await GetEvents();
        var eventHandlers = await GetEventHandlers();

        return new UdupResponse(
            events,
            Enumerable.Select<EventHandler, EventHandler>(eventHandlers, _ => new EventHandler(
                _.Handler, _.Events)).ToList()
        );
    }

    public async Task<List<EventWithTrace>> GetEvents()
    {
        var solution = await workspace.OpenSolutionAsync(@"C:\P\Edu\udup\Udup.sln");
        var events = new List<IdAndName>();
        var eventSources = new List<(IdAndName Event, IdAndName Source)>();
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
            .GroupBy(_ => _.Event.Id)
            .ToDictionary(
                _ => _.Key,
                _ => _
                    .Select(_ => _.Source)
                    .ToList());

        return events.Select(e =>
            new EventWithTrace(e, eventSourcesDictionary.TryGetValue(e.Id, out var value) ? value : [])).ToList();
    }

    private async Task<List<(IdAndName Event, IdAndName Source)>> GetEventSource(Compilation? compilation, Document document)
    {
        var tree = await document.GetSyntaxTreeAsync();
        var root = await tree.GetRootAsync();
        var semanticModel = compilation.GetSemanticModel(tree);

        var result = root.DescendantNodes().OfType<ObjectCreationExpressionSyntax>()
            .Where(_ => IsUdupEvent(semanticModel.GetSymbolInfo(_).Symbol.ContainingType))
            .ToArray();

        var list = new List<(IdAndName Event, IdAndName Source)>();
        foreach (var node in result)
        {
            var stringBuilder = BuildPath(node, semanticModel);

            list.Add((new IdAndName(semanticModel.GetSymbolInfo(node).Symbol.ContainingType.Name), new IdAndName(CreateIdentifier(node),stringBuilder)));
        }

        return list;
    }

    private string CreateIdentifier(ObjectCreationExpressionSyntax node)
    {
        return $"creation{node.Span.Start}e{node.Span.End}";
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

        if (node is GlobalStatementSyntax expression)
        {
            stringBuilder.Append($"EndpointTODOs{expression.Span.Start}e{expression.Span.End}");
        }

        return stringBuilder.ToString();
    }

    private async Task<IdAndName[]> GetEventsFromSyntaxAndSemantic(Compilation? compilation, Document document)
    {
        var tree = await document.GetSyntaxTreeAsync();
        var root = tree.GetRoot();
        var semanticModel = compilation.GetSemanticModel(tree);

        return root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>()
            .Select(syntax => semanticModel.GetDeclaredSymbol(syntax))
            .Where(symbol => symbol != null)
            .Where(symbol => symbol!.IsImplicitlyDeclared is false)
            .Where(IsUdupEvent)
            .Select(symbol => new IdAndName(symbol.Name))
            .ToArray();
    }

    private static bool IsUdupEvent(INamedTypeSymbol symbol)
    {
        return symbol.Interfaces.Any(@interface =>
            GetFullName(@interface) == $"{typeof(IUdupMessage).FullName}");
    }

    private static string GetFullName(INamedTypeSymbol @interface)
    {
        return $"{string.Join('.', @interface.ContainingNamespace.ConstituentNamespaces)}.{@interface.Name}";
    }
}