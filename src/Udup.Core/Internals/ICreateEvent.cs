using Microsoft.CodeAnalysis;
using Udup.Abstractions;

namespace Udup.Core.Internals;

public interface IAmCreatingEvent
{
    Usage[] FindUsagesHere(SyntaxNode root, SemanticModel semanticModel);
}