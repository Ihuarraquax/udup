using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;

namespace RoslynPlayground;

public class RoslynPlayground
{
    [Fact]
    public void Test()
    {
        MSBuildLocator.RegisterDefaults();
        
        var workspace = MSBuildWorkspace.Create();
        workspace.LoadMetadataForReferencedProjects = true;
        
        workspace.WorkspaceFailed += (sender, args) =>
        {
            Console.WriteLine(args.Diagnostic.Message);
        };
    }
}