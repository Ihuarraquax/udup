using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Udup.Core.Roslyn.Internals;

public record EventTrace(IdAndName Event, IdAndName Name);

public class Gatherer_EventTraces
{
    internal static async Task<List<EventTrace>> GatherEventsTraces(Solution solution)
    {
        var eventTraces = new List<EventTrace>();
        foreach (var project in solution.Projects)
        {
            var compilation = await project.GetCompilationAsync();

            foreach (var document in project.Documents)
            {
                eventTraces.AddRange(await GetEventTraces(compilation, document));
            }
        }

        return eventTraces;
    }

    internal static async Task<List<EventTrace>> GetEventTraces(Compilation? compilation,
        Document document)
    {
        var tree = await document.GetSyntaxTreeAsync();
        var root = await tree.GetRootAsync();
        var semanticModel = compilation.GetSemanticModel(tree);

        var result = root.DescendantNodes().OfType<ObjectCreationExpressionSyntax>()
            .Where(_ => semanticModel.GetSymbolInfo(_).Symbol.ContainingType.IsUdupEvent())
            .ToArray();

        var list = new List<EventTrace>();
        foreach (var node in result)
        {
            var stringBuilder = BuildPath(node, semanticModel);

            list.Add(new EventTrace(new IdAndName(semanticModel.GetSymbolInfo(node).Symbol.ContainingType.Name),
                new IdAndName(node.CreateIdentifier(), stringBuilder)));
        }

        return list;
    }

    internal static string BuildPath(SyntaxNode node, SemanticModel semanticModel)
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
}