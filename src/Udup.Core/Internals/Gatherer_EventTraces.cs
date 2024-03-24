using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Udup.Abstractions;

namespace Udup.Core.Internals;

public class Gatherer_EventTraces : CSharpSyntaxWalker
{
    public readonly List<EventTrace> EventTraces = [];
    public List<(SyntaxNode root, SemanticModel semanticModel)> rootsWithSemantics;

    public Gatherer_EventTraces(List<(SyntaxNode root, SemanticModel semanticModel)> rootsWithSemantics)
    {
        this.rootsWithSemantics = rootsWithSemantics;
    }

    public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
    {
        Process(node);
    }

    private void Process(ObjectCreationExpressionSyntax node)
    {
        var symbol = semanticModel.GetSymbolInfo(node).Symbol;
        if (symbol is null) return;
        if (symbol.ContainingType.IsUdupEvent() is false) return;

        EventUsages eventUsages = BuildEventUsages(node);

        EventTraces.Add(new EventTrace(
            new IdAndName(symbol.ContainingType.Name),
            new IdAndName(CreateIdentifier(node), "")));
    }

    private EventUsages BuildEventUsages(ObjectCreationExpressionSyntax node)
    {
        var eventParent = FindEventParent(node);

        var eventParentUsages = FindEventParentUsages(eventParent);

        return null;
    }

    private object FindEventParentUsages(IAmCreatingEvent eventParent)
    {
        foreach (var VARIABLE in COLLECTION)
        {
            
        }
        // if Method
        // look for usages


        return null;
    }

    private IAmCreatingEvent FindEventParent(SyntaxNode node)
    {
        while (node.Parent != null)
        {
            if (node.Parent is PropertyDeclarationSyntax)
                return new SomethingCreatingEvent(node.Parent);

            if (node.Parent is MethodDeclarationSyntax)
                return new SomethingCreatingEvent(node.Parent);

            if (IsMinimalApiEndpoint(node.Parent))
            {
                return new SomethingCreatingEvent(node.Parent);
            }


            node = node.Parent;
        }

        throw new InvalidOperationException("Could not find parent of event");
    }

    private bool IsMinimalApiEndpoint(SyntaxNode nodeParent)
    {
        if (nodeParent is InvocationExpressionSyntax
            {
                Expression: MemberAccessExpressionSyntax
                {
                    Expression: IdentifierNameSyntax identifierName
                }
            })
        {
            var symbol = ModelExtensions.GetSymbolInfo(semanticModel, identifierName);
            return IsWebApplication(symbol);
        }

        return false;
    }

    // private static string BuildPath(SyntaxNode? node, SemanticModel semanticModel)
    // {
    //     var stringBuilder = new StringBuilder();
    //
    //     if (node == null)
    //     {
    //         return "";
    //     }
    //
    //     if (node is InvocationExpressionSyntax
    //         {
    //             Expression: MemberAccessExpressionSyntax
    //             {
    //                 Expression: IdentifierNameSyntax identifierName
    //             } memberAccess
    //         } invocation)
    //     {
    //         var symbol = ModelExtensions.GetSymbolInfo(semanticModel, identifierName);
    //         if (IsWebApplication(symbol))
    //         {
    //             var method = ToHttpMethod(memberAccess.Name.Identifier.Text);
    //             var path = invocation.ArgumentList.Arguments[0].Expression.ToString();
    //
    //             stringBuilder.Append($"{method} {path}");
    //             return stringBuilder.ToString();
    //         }
    //     }
    //
    //     stringBuilder.Append(BuildPath(node.Parent, semanticModel));
    //
    //     switch (node)
    //     {
    //         case MethodDeclarationSyntax methodDeclaration:
    //             stringBuilder.Append(methodDeclaration.Identifier.Text);
    //             stringBuilder.Append("()");
    //             break;
    //         case BaseTypeDeclarationSyntax typeDeclaration:
    //             stringBuilder.Append(typeDeclaration.Identifier.Text);
    //             stringBuilder.Append('.');
    //             break;
    //     }
    //
    //     return stringBuilder.ToString();
    // }

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

public record EventUsages
{
    List<Usages> DirectUsages;
}

internal class Usages
{
    private string Path;

    List<Usages> IndirectUsages;
}