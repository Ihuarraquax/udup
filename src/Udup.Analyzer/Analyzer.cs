using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Udup.Analyzer;

public class Analyzer
{
    public List<UdupType> Analyze(List<(SyntaxNode root, SemanticModel semanticModel)> data)
    {
        var walker = new Walker([], data.First().semanticModel);
        walker.Visit(data.First().root);
        
        return walker.UdupTypes;
    }
}

public class Walker(List<UdupType> udupTypes, SemanticModel semanticModel): CSharpSyntaxWalker
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
        
        foreach (var method in symbolInfo.GetMembers()
                     .Where(member => member.Kind == SymbolKind.Method)
                     .Where(member => member.IsImplicitlyDeclared == false)
                 )
        {
            udupType.Methods.Add(method.ToDisplayString());
        }
        
        UdupTypes.Add(udupType);
        base.VisitClassDeclaration(node);
    }
}

public class UdupType
{
    public string Name { get; set; }
    public List<string> Interfaces { get; set; } = [];
    public List<string> Methods { get; set; } = [];
}