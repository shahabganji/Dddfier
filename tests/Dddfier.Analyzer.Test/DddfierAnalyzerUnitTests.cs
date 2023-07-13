using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Microsoft.CodeAnalysis.Text;
using VerifyCS = Dddfier.Analyzer.Test.Verifiers.CSharpCodeFixVerifier<
    Dddfier.Analyzer.DddfierAnalyzer,
    Dddfier.Analyzer.DddfierAnalyzerCodeFixProvider>;

namespace Dddfier.Analyzer.Test;

[TestClass]
public class DddfierAnalyzerUnitTest
{
    //No diagnostics expected to show up
    [TestMethod]
    public async Task No_Source_No_Diagnostics()
    {
        // Arrange
        const string test = @"";

        // Assert
        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    //Diagnostic and CodeFix both triggered and checked for
    [TestMethod]
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

        var analyserTest = new CSharpAnalyzerTest<DddfierAnalyzer, MSTestVerifier>()
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
    [TestMethod]
    public async Task Having_Id_Property_With_Primitive_Type_Is_A_Sign_Of_Primitive_Obsession()
    {
        // Arrange
        const string test =
            $$"""
                namespace TestSample
                {
                    public class TestClass
                    {
                        public int #4|Id { get; set; }
                    }
                }
                """;

        var expected = new DiagnosticResult(DddfierAnalyzer.DiagnosticId, DiagnosticSeverity.Error)
            ;
        
        // Assert
        await VerifyCS.VerifyAnalyzerAsync(test,expected);
    }
}
