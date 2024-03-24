using Microsoft.CodeAnalysis;

namespace Udup.Core.Internals;

class SomethingCreatingEvent : IAmCreatingEvent
{
    private readonly SyntaxNode nodeParent;

    public SomethingCreatingEvent(SyntaxNode nodeParent)
    {
        this.nodeParent = nodeParent;
    }

    public bool ThisUsageIsMe()
    {
        return true;
    }
}