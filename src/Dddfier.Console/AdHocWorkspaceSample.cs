using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Dddfier.Console;

public static class AdHocWorkspaceSample
{
    public static Task Execute()
    {
        var adhoc = new AdhocWorkspace();

        var solutionInfo = SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Default);

        adhoc.AddSolution(solutionInfo);

        var projectInfo = ProjectInfo.Create(ProjectId.CreateNewId(), VersionStamp.Default,
            "Sample", "Sample.dll", "C#");
        adhoc.AddProject(projectInfo);

        var documentInfo = DocumentInfo.Create(DocumentId.CreateNewId(projectInfo.Id), "Test.cs",
            sourceCodeKind: SourceCodeKind.Regular);
        adhoc.AddDocument(documentInfo);
        var sourceText = SourceText.From(
            """
            public class Sample { }
            """);
        adhoc.AddDocument(projectInfo.Id, "Sample.cs", sourceText);

        foreach (var document in adhoc.CurrentSolution.Projects.SelectMany(x=>x.Documents))
        {
            System.Console.WriteLine(document.Id);
        }
        
        return Task.CompletedTask;
    }
}
