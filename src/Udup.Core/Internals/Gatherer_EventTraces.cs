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
                new IdAndName(CreateIdentifier(node), stringBuilder)));
        }

        return list;
    }

    private static string BuildPath(SyntaxNode? node, SemanticModel semanticModel)
    {
        var stringBuilder = new StringBuilder();

        switch (node)
        {
            case null:
                return "";
            case InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax { Expression: IdentifierNameSyntax identifierName } memberAccess } invocation:
            {
                var symbol = semanticModel.GetSymbolInfo(identifierName);
                if (IsWebApplication(symbol))
                {
                    var method = ToHttpMethod(memberAccess.Name.Identifier.Text);
                    var path  = invocation.ArgumentList.Arguments[0].Expression.ToString();
                        
                    stringBuilder.Append($"{method} {path}");
                    return stringBuilder.ToString();
                }

                break;
            }
        }

        stringBuilder.Append(BuildPath(node.Parent, semanticModel));

        switch (node)
        {
            case MethodDeclarationSyntax methodDeclaration:
                stringBuilder.Append(methodDeclaration.Identifier.Text);
                stringBuilder.Append("()");
                break;
            case BaseTypeDeclarationSyntax typeDeclaration:
                stringBuilder.Append(typeDeclaration.Identifier.Text);
                stringBuilder.Append('.');
                break;
        }

        return stringBuilder.ToString();
    }

    private static bool IsWebApplication(SymbolInfo node)
    {
        return node.Symbol switch
        {
            IParameterSymbol parameter => parameter.Type.ToDisplayString() ==
                                          "Microsoft.AspNetCore.Builder.WebApplication",
            ILocalSymbol local => local.Type.ToDisplayString() ==
                                  "Microsoft.AspNetCore.Builder.WebApplication?",
            _ => false
        };
    }

    private static string CreateIdentifier(ObjectCreationExpressionSyntax? node)
    {
        return $"creation{node.Span.Start}e{node.Span.End}";
    }

    private static string ToHttpMethod(string text)
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