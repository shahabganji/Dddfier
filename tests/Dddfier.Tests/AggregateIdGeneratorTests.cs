using Dddifier.Generators;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Dddfier.Tests;

public class AggregateIdGeneratorTests
{
    [Fact]
    public void WithIdOfAttribute_Is_Generated()
    {
        // Arrange
        var source = string.Empty;

        // Act
        var compilation = GetGeneratedOutput(source);

        // Assert
        compilation.Should().NotBeNull();
        var generatedCode = compilation!.SyntaxTrees.Single(st => st.FilePath.EndsWith("WithIdOfAttribute.g.cs"))
            .ToString();
        generatedCode.Should().Be(
            """
            // <auto-generated />

            namespace Dddfier.GeneratedCode.Attributes;

            public class WithIdOfAttribute<T> : global::System.Attribute
            {
                public string PropertyName { get; set; } = "Id";
                public global::Dddfier.GeneratedCode.SetterModifier SetterModifier { get; set; } = global::Dddfier.GeneratedCode.SetterModifier.Private;
            }
            """
        );
    }

    [Fact]
    public void SetterModifier_Is_Generated()
    {
        // Arrange
        var source = string.Empty;

        // Act
        var compilation = GetGeneratedOutput(source);

        // Assert
        compilation.Should().NotBeNull();
        var generatedCode = compilation!.SyntaxTrees.Single(st => st.FilePath.EndsWith("SetterModifierEnum.g.cs"))
            .ToString();
        generatedCode.Should().Be(
            """
            // <auto-generated />
            
            namespace Dddfier.GeneratedCode;
            
            public enum SetterModifier
            {
                Public,
                Private,
                Protected,
            }
            """
        );
    }

    private static Compilation? GetGeneratedOutput(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        var compilation = CSharpCompilation.Create("DddfierSourceGeneratorTests",
            new[] { syntaxTree });

        var generator = new AggregateIdGenerator();

        CSharpGeneratorDriver.Create(generator)
            .RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        return outputCompilation;
    }
}
