using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using EventHandler = Udup.Abstractions.EventHandler;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Udup.SourceGenerators
{
    [Generator]
    public class HelloSourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            List<EventHandler> eventHandlers = new List<EventHandler>();
            foreach (var syntaxTree in context.Compilation.SyntaxTrees)
            {
                var semanticModel = context.Compilation.GetSemanticModel(syntaxTree);
                // var visitior = new Gatherer_EventHandlers(semanticModel);
                // eventHandlers.AddRange(visitior.GetWithWalker(syntaxTree));
            }

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(eventHandlers);

            var source = GenerateSource(json);

            context.AddSource("hello.g.cs", source.GetText(Encoding.UTF8));
            // Code generation goes here
        }

        private static CompilationUnitSyntax GenerateSource(string json)
        {
            return CompilationUnit()
                .WithMembers(
                    SingletonList<MemberDeclarationSyntax>(
                        ClassDeclaration("StaticUdupResponse")
                            .WithModifiers(
                                TokenList(
                                    Token(SyntaxKind.PublicKeyword)))
                            .WithMembers(
                                SingletonList<MemberDeclarationSyntax>(
                                    FieldDeclaration(
                                            VariableDeclaration(
                                                    PredefinedType(
                                                        Token(SyntaxKind.StringKeyword)))
                                                .WithVariables(
                                                    SingletonSeparatedList<VariableDeclaratorSyntax>(
                                                        VariableDeclarator(
                                                                Identifier("UdupJson"))
                                                            .WithInitializer(
                                                                EqualsValueClause(
                                                                    LiteralExpression(
                                                                        SyntaxKind.StringLiteralExpression,
                                                                        Literal(json)))))))
                                        .WithModifiers(
                                            TokenList(
                                                new[]
                                                {
                                                    Token(SyntaxKind.PublicKeyword),
                                                    Token(SyntaxKind.StaticKeyword)
                                                }))))))
                .NormalizeWhitespace();
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }
        
        public class MainSyntaxReceiver : ISyntaxReceiver
        {
            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
            }
        }
    }
}