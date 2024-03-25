using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Udup.Abstractions;
using Udup.Core.Internals;
using EventHandler = Udup.Abstractions.EventHandler;

namespace Udup.Core;

public class SolutionReader
{
    private readonly string solutionPath;
    private readonly MSBuildWorkspace workspace;

    public SolutionReader(string solutionPath)
    {
        this.solutionPath = solutionPath;
        MSBuildLocator.RegisterDefaults();

        workspace = MSBuildWorkspace.Create();
        workspace.LoadMetadataForReferencedProjects = true;

        workspace.WorkspaceFailed += (sender, args) => { Console.WriteLine(args.Diagnostic.Message); };
    }
    
    public async Task<List<(SyntaxNode root, SemanticModel semanticModel)>> GatherSolutionNodesWithSemantics()
    {
        var solution = await OpenSolutionAsync();
        
        var rootsWithSemantics = new List<(SyntaxNode root, SemanticModel semanticModel)>();

        foreach (var project in solution.Projects)
        {
            var compilation = await project.GetCompilationAsync();

            foreach (var document in project.Documents)
            {
                var tree = await document.GetSyntaxTreeAsync();
                var root = await tree.GetRootAsync();
                
                var semanticModel = compilation.GetSemanticModel(tree);
                rootsWithSemantics.Add((root, semanticModel));
            }
        }

        return rootsWithSemantics;
    }

    private async Task<Solution> OpenSolutionAsync()
    {
        return await workspace.OpenSolutionAsync(solutionPath);
    }
}