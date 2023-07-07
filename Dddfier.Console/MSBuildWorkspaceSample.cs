using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;

namespace Dddfier.Console;

public static class MSBuildWorkspaceSample
{
    public static async Task Execute()
    {
        
        const string path = @"C:\Users\saeed.ganji\.dev\ringana\RightsApi\Ringana.Rights.sln";

        MSBuildLocator.RegisterDefaults();

        using var workspace = MSBuildWorkspace.Create();
        var solution = await workspace.OpenSolutionAsync(path);

        foreach (var project in solution.Projects)
        {
            System.Console.WriteLine(project.AssemblyName);
        }

        var document = solution.Projects.SelectMany(p => p.Documents.Where(m => m.Name == "IApiMarker.cs")).Single();

        var sourceText = SourceText.From(
            """
    using System.Diagnostics.CodeAnalysis;

    namespace Ringana.Rights.Api;

    [SuppressMessage("Design", "CA1040:Avoid empty interfaces")]
    public interface IApiMarker
    {
    }

    public interface IComeFromSourceGenerators
    {
    }
    """);

// everything related to this newDocument is new instances of their type, e.g. new project, new solution and so on.
        var newDocument = document.WithText(sourceText);

        var changedApplied = workspace.TryApplyChanges(newDocument.Project.Solution);

        System.Console.WriteLine(changedApplied ? "Applied" : "Something went wrong");
    }
    
}
