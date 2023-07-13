using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Xunit;

namespace Dddfier.Analyzer.Test;

public class DddfierAnalyzerUnitTest
{
    //No diagnostics expected to show up
    [Fact]
    public async Task No_Source_No_Diagnostics()
    {
        // Arrange
        const string test = @"";

        var analyserTest = new CSharpAnalyzerTest<DddfierAnalyzer, XUnitVerifier>
        {
            TestState =
            {
                Sources = { test }
            }
        };

        // Assert
        await analyserTest.RunAsync();

    }

    //Diagnostic and CodeFix both triggered and checked for
    [Fact]
    public async Task No_Id_No_Diagnostics()
    {
        // Arrange
        const string test =
                $$"""
                namespace TestSample
                {
                    public class TestClass
                    {
                        public int Value { get; set; }
                    }
                }
                """;

        var analyserTest = new CSharpAnalyzerTest<DddfierAnalyzer, XUnitVerifier>
        {
            TestState =
            {
                Sources = { test }
            }
        };
        
        // Assert
        await analyserTest.RunAsync();
    }
    
    //Diagnostic and CodeFix both triggered and checked for
    [Fact]
    public async Task Having_Id_Property_With_Primitive_Type_Is_A_Sign_Of_Primitive_Obsession()
    {
        // Arrange
        const string test =
            $$"""
                namespace TestSample
                {
                    public class TestClass
                    {
                        public int Id { get; set; }
                    }
                }
                """;

        var expected = new DiagnosticResult(DddfierAnalyzer.DiagnosticId, DiagnosticSeverity.Error)
            .WithLocation(5, 9);
            
        
        var analyserTest = new CSharpAnalyzerTest<DddfierAnalyzer, XUnitVerifier>
        {
            TestState =
            {
                Sources = { test }
            },
            ExpectedDiagnostics = { expected }
        };

        // Assert
        await analyserTest.RunAsync();
    }
}
