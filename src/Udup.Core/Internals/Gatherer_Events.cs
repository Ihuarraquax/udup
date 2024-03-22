using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Udup.Abstractions;

namespace Udup.Core.Internals;

public class Gatherer_Events : CSharpSyntaxWalker
{
    private readonly SemanticModel? semanticModel;
    public readonly List<IdAndName> Events = new();

    public Gatherer_Events(SemanticModel? semanticModel)
    {
        this.semanticModel = semanticModel;
    }

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        Process(node);
    }

    public override void VisitRecordDeclaration(RecordDeclarationSyntax node)
    {
        Process(node);
    }

    public void Process(BaseTypeDeclarationSyntax node)
    {
        var symbol = semanticModel.GetDeclaredSymbol(node);
        if (symbol is null) return;
        if (symbol.IsImplicitlyDeclared) return;

        if (symbol.IsUdupEvent())
        {
            Events.Add(BuildEventResponse(symbol));
        }
    }

    private static IdAndName BuildEventResponse(INamedTypeSymbol symbol)
    {
        return new IdAndName(symbol.Name);
    }
}