using Dddfier.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Dddfier.Tests;

public static class CSharpTestHelper
{
    public static Compilation GetGeneratedOutput(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(_ => !_.IsDynamic && !string.IsNullOrWhiteSpace(_.Location))
            .Select(_ => MetadataReference.CreateFromFile(_.Location))
            .Concat(new[] {MetadataReference.CreateFromFile(typeof(AggregateIdGenerator).Assembly.Location)});
        
        var compilation = CSharpCompilation.Create("DddfierSourceGeneratorTests",
            new[] { syntaxTree }, references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generator = new AggregateIdGenerator();

        // var originalTreeCount = compilation.SyntaxTrees.Length;

        CSharpGeneratorDriver.Create(generator)
            .RunGenerators(compilation)
            .RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        return outputCompilation;
    }
}
