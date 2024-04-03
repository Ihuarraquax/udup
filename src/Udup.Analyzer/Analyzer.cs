using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;

namespace Udup.Analyzer;

public class Analyzer(Solution solution)
{
    public List<UdupType> Analyze(List<(SyntaxNode root, SemanticModel semanticModel)> data)
    {
        var walker = new Walker([], data.First().root, data.First().semanticModel, solution);
        walker.Visit(data.First().root);

        return walker.UdupTypes;
    }
}

public class Walker(List<UdupType> udupTypes, SyntaxNode root, SemanticModel semanticModel, Solution solution) : CSharpSyntaxWalker
{
    public readonly List<UdupType> UdupTypes = udupTypes;

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        var symbolInfo = semanticModel.GetDeclaredSymbol(node);

        var udupType = new UdupType { Name = symbolInfo.ToDisplayString() };

        //Add interfaces
        foreach (var @interface in symbolInfo.Interfaces)
        {
            udupType.Interfaces.Add(@interface.ToDisplayString());
        }
        var TypeUsages = SymbolFinder.FindCallersAsync(symbolInfo, solution).Result;
        var referenced = SymbolFinder.FindReferencesAsync(symbolInfo, solution).Result;
        var result2 = TypeUsages.Select(
            c =>
                new
                {
                    Who = c.CallingSymbol.ToDisplayString(),
                    Where = c.Locations.Select(l => new { l.SourceSpan }),
                });
        
        udupType.UsedIn = result2.Select(_ => _.Who).ToList();
        
        AddMethods(symbolInfo, udupType);

        UdupTypes.Add(udupType);
        base.VisitClassDeclaration(node);
    }

    private void AddMethods(INamedTypeSymbol symbolInfo, UdupType udupType)
    {
        foreach (var member in symbolInfo.GetMembers()
                     .Where(member => member.Kind == SymbolKind.Method)
                     .Where(member => member.IsImplicitlyDeclared == false)
                )
        {
            var methodSymbol = (IMethodSymbol)member;

            var callers = SymbolFinder.FindCallersAsync(methodSymbol, solution).Result;

            var result = callers.Select(
                c =>
                    new
                    {
                        Who = c.CallingSymbol.ToDisplayString(),
                        Where = c.Locations.Select(l => new { l.SourceSpan }),
                    });

            var udupMethod = new UdupMethod(methodSymbol, result.Select(_ => _.Who).ToList());
            udupType.Methods.Add(udupMethod);
        }
    }
}

public class UdupType
{
    public string Name { get; set; }

    public List<string> Interfaces { get; set; } = [];

    public List<UdupMethod> Methods { get; set; } = [];

    public List<string> UsedIn { get; set; } = [];
}

public class UdupMethod
{
    public UdupMethod(IMethodSymbol methodSymbol, List<string> references)
    {
        Name = methodSymbol.ToDisplayString();

        UsedIn = references;
    }

    public string Name { get; set; }

    public List<string> UsedIn { get; set; }
}