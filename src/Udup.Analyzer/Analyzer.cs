using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;

namespace Udup.Analyzer;

public class Analyzer(Solution solution)
{
    public List<UdupType> Analyze(List<(SyntaxNode root, SemanticModel semanticModel)> data)
    {
        var walker = new Walker([], data.First().semanticModel, solution);
        walker.Visit(data.First().root);
        
        return walker.UdupTypes;
    }
}

public class Walker(List<UdupType> udupTypes, SemanticModel semanticModel, Solution solution): CSharpSyntaxWalker
{
    public readonly List<UdupType> UdupTypes = udupTypes;

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        var symbolInfo = semanticModel.GetDeclaredSymbol(node);
        
        var udupType = new UdupType { Name = symbolInfo.ToDisplayString()};

        foreach (var @interface in symbolInfo.Interfaces)
        {
            udupType.Interfaces.Add(@interface.ToDisplayString());
        }
        
        foreach (var member in symbolInfo.GetMembers()
                     .Where(member => member.Kind == SymbolKind.Method)
                     .Where(member => member.IsImplicitlyDeclared == false)
                 )
        {
            var methodSymbol = (IMethodSymbol) member;
            var references = SymbolFinder.FindReferencesAsync(methodSymbol, solution).Result;
            var udupMethod = new UdupMethod(methodSymbol, references);
            udupType.Methods.Add(udupMethod);
        }
        
        UdupTypes.Add(udupType);
        base.VisitClassDeclaration(node);
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
    public UdupMethod(IMethodSymbol methodSymbol, IEnumerable<ReferencedSymbol> references)
    {
        Name = methodSymbol.ToDisplayString();
        
        UsedIn = references.SelectMany(reference => reference.Locations)
            .Select(location => location.Location)
            .ToList();
    }

    public string Name { get; set; }
    public List<Location> UsedIn { get; set; }
}