// See https://aka.ms/new-console-template for more information


using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Formatting;
using Dddfier.Console;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;

const string path = @"C:\Users\saeed.ganji\.dev\ringana\RightsApi\Ringana.Rights.sln";

MSBuildLocator.RegisterDefaults();

using var workspace = MSBuildWorkspace.Create();
var solution = await workspace.OpenSolutionAsync(path);

foreach (var project in solution.Projects)
{
    Console.WriteLine(project.AssemblyName);
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

Console.WriteLine(changedApplied ? "Applied" : "Something went wrong");


var tree = CSharpSyntaxTree.ParseText(
    // lang=csharp
    """
    class Program
    {
        static void Main()
        {
            if(true)
                Console.WriteLine("It was true!");
                
            if(false)
                Console.WriteLine("OMG! how'd we get here!");
        }
    }
    """);


#region syntax rewriter : 

// var root = tree.GetRoot();
// var rewriter = new MyIfRewriter();
// var newRoot = rewriter.Visit(root);
//
// Console.WriteLine(newRoot.ToFullString());

#endregion

#region syntax Walker : 

// var myWalker = new MyWalker();
// var root = tree.GetRoot();
// myWalker.Visit(root);
//
//
// Console.WriteLine(myWalker.Sb.ToString());

#endregion

#region syntax tree api: 
// var tree = CSharpSyntaxTree.ParseText(
//     // lang=csharp
//     """
//     class c
//     {
//         void Method()
//         {
//         }
//     }
//     """);
//


// var root = tree.GetRoot();
// var method = root.DescendantNodes().OfType<MethodDeclarationSyntax>().First();
//
// var returnType = SyntaxFactory.ParseTypeName("string");
// var newMethod = method.WithReturnType(returnType);

//var diagnostics=  tree.GetDiagnostics();

//var errors = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error);

//Console.WriteLine(errors.First());

// Console.WriteLine(newMethod.ToString());

#endregion
