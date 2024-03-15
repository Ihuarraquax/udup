using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Udup.Abstractions;

namespace Udup.Core.Internals;

public class Gatherer_EventTraces
{
    public static async Task<List<EventTrace>> Get(SyntaxTree tree, SemanticModel? semanticModel)
    {
        var root = await tree.GetRootAsync();

        ObjectCreationExpressionSyntax?[] result = root.DescendantNodes().OfType<ObjectCreationExpressionSyntax>()
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

    internal static string BuildPath(SyntaxNode? node, SemanticModel semanticModel)
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